using System;
using System.Data;
using System.Data.SqlClient;
using Model;

namespace BusinessLogic
{
    public class FrontDeskOperationsService
    {
        private readonly string connectionString = DatabaseConfig.ConnectionString;

        public DataTable SearchCheckIn(string keyword)
        {
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(
                @"SELECT r.ReservationId, g.GuestName, rm.RoomNumber,
                         r.CheckInDate, r.ReservationStatus
                  FROM dbo.Reservations r
                  INNER JOIN dbo.Guests g ON g.GuestId = r.GuestId
                  INNER JOIN dbo.Rooms rm ON rm.RoomId = r.RoomId
                  WHERE g.GuestName LIKE @guestName
                    AND r.ReservationStatus IN ('Pending','Confirmed')
                  ORDER BY r.CheckInDate, r.ReservationId", connection))
            {
                command.Parameters.Add("@guestName", SqlDbType.NVarChar, 255).Value = "%" + (keyword ?? "").Trim() + "%";
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    adapter.Fill(table);
            }
            return table;
        }

        public bool CheckIn(int reservationId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(
                @"UPDATE dbo.Reservations
                  SET ReservationStatus = 'Checked In'
                  WHERE ReservationId = @reservationId
                    AND ReservationStatus IN ('Pending','Confirmed')", connection))
            {
                command.Parameters.Add("@reservationId", SqlDbType.Int).Value = reservationId;
                connection.Open();
                return command.ExecuteNonQuery() == 1;
            }
        }

        public DataTable GetAvailableRooms(DateTime checkIn, DateTime checkOut, string roomType)
        {
            if (checkOut.Date <= checkIn.Date)
                throw new InvalidOperationException("Check-out must be later than check-in.");

            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(
                @"SELECT rm.RoomId, rm.RoomNumber, rm.RoomType, rm.RoomRate,
                         'Available' AS RoomStatus
                  FROM dbo.Rooms rm
                  WHERE rm.RoomType = @roomType
                    AND ISNULL(rm.RoomStatus, N'Available') = N'Available'
                    AND NOT EXISTS (
                       SELECT 1 FROM dbo.Reservations r
                       WHERE r.RoomId = rm.RoomId
                         AND r.ReservationStatus IN ('Pending','Confirmed','Checked In')
                         AND r.CheckInDate < @checkOut
                         AND r.CheckOutDate > @checkIn)
                  ORDER BY rm.RoomNumber", connection))
            {
                command.Parameters.Add("@roomType", SqlDbType.NVarChar, 100).Value = roomType;
                command.Parameters.Add("@checkIn", SqlDbType.Date).Value = checkIn.Date;
                command.Parameters.Add("@checkOut", SqlDbType.Date).Value = checkOut.Date;
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    adapter.Fill(table);
            }
            return table;
        }

        public BillingInfo GetCheckedInBilling(string keyword)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(BillingSelectSql +
                @" WHERE g.GuestName LIKE @guestName
                     AND r.ReservationStatus = 'Checked In'
                   ORDER BY r.ReservationId DESC", connection))
            {
                command.Parameters.Add("@guestName", SqlDbType.NVarChar, 255).Value = "%" + (keyword ?? "").Trim() + "%";
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    BillingInfo billing = ReadBilling(reader);
                    if (reader.Read())
                        throw new InvalidOperationException(
                            "More than one checked-in reservation matched. Search using the full guest name or reservation ID.");
                    return billing;
                }
            }
        }

        public BillingInfo GetCheckedInBillingByReservationId(int reservationId)
        {
            if (reservationId <= 0)
                throw new InvalidOperationException("Enter a valid reservation ID.");

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(BillingSelectSql +
                @" WHERE r.ReservationId = @reservationId
                     AND r.ReservationStatus = 'Checked In'", connection))
            {
                command.Parameters.Add("@reservationId", SqlDbType.Int).Value = reservationId;
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    return ReadBilling(reader);
                }
            }
        }

        public PaymentReceiptInfo CompleteCheckout(
            BillingInfo billing,
            string method,
            decimal amountReceived,
            string processedBy)
        {
            if (billing == null)
                throw new InvalidOperationException("Select a checked-in guest first.");
            if (method != "Cash" && method != "GCash" && method != "Card")
                throw new InvalidOperationException("Select Cash, GCash, or Card.");
            if (amountReceived < billing.TotalBill)
                throw new InvalidOperationException("Amount received must cover the total bill.");

            decimal changeAmount = method == "Cash"
                ? Math.Max(0m, amountReceived - billing.TotalBill)
                : 0m;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        decimal total;
                        using (SqlCommand read = new SqlCommand(
                            @"SELECT rm.RoomRate, r.CheckInDate, r.CheckOutDate
                              FROM dbo.Reservations r WITH (UPDLOCK, HOLDLOCK)
                              INNER JOIN dbo.Rooms rm ON rm.RoomId = r.RoomId
                              WHERE r.ReservationId = @id
                                AND r.ReservationStatus = 'Checked In'
                                AND NOT EXISTS (
                                   SELECT 1 FROM dbo.Payments p
                                   WHERE p.ReservationId = r.ReservationId
                                     AND p.PaymentStatus = 'Paid')", connection, transaction))
                        {
                            read.Parameters.Add("@id", SqlDbType.Int).Value = billing.ReservationId;
                            using (SqlDataReader reader = read.ExecuteReader())
                            {
                                if (!reader.Read())
                                    throw new InvalidOperationException(
                                        "This reservation is no longer checked in, or is already paid.");
                                decimal rate = Convert.ToDecimal(reader["RoomRate"]);
                                DateTime start = Convert.ToDateTime(reader["CheckInDate"]);
                                DateTime end = Convert.ToDateTime(reader["CheckOutDate"]);
                                total = rate * Math.Max(1, (end.Date - start.Date).Days);
                            }
                        }

                        if (total != billing.TotalBill)
                            throw new InvalidOperationException(
                                "The billing amount has changed. Search the guest again.");

                        PaymentReceiptInfo receipt;
                        using (SqlCommand payment = new SqlCommand(
                            @"INSERT INTO dbo.Payments
                              (ReservationId, PaymentAmount, PaymentMethod, PaymentStatus,
                               AmountReceived, ChangeAmount, ProcessedBy)
                              OUTPUT INSERTED.PaymentId, INSERTED.PaymentDate
                              VALUES (@id, @amount, @method, 'Paid',
                                      @received, @change, @processedBy)", connection, transaction))
                        {
                            payment.Parameters.Add("@id", SqlDbType.Int).Value = billing.ReservationId;

                            SqlParameter amount = payment.Parameters.Add("@amount", SqlDbType.Decimal);
                            amount.Precision = 18;
                            amount.Scale = 2;
                            amount.Value = total;

                            payment.Parameters.Add("@method", SqlDbType.NVarChar, 30).Value = method;

                            SqlParameter received = payment.Parameters.Add("@received", SqlDbType.Decimal);
                            received.Precision = 18;
                            received.Scale = 2;
                            received.Value = amountReceived;

                            SqlParameter change = payment.Parameters.Add("@change", SqlDbType.Decimal);
                            change.Precision = 18;
                            change.Scale = 2;
                            change.Value = changeAmount;

                            payment.Parameters.Add("@processedBy", SqlDbType.NVarChar, 100).Value =
                                string.IsNullOrWhiteSpace(processedBy) ? "Front Desk" : processedBy.Trim();

                            using (SqlDataReader paymentReader = payment.ExecuteReader())
                            {
                                if (!paymentReader.Read())
                                    throw new InvalidOperationException("Payment could not be recorded.");

                                receipt = new PaymentReceiptInfo
                                {
                                    PaymentId = Convert.ToInt32(paymentReader["PaymentId"]),
                                    PaymentDate = Convert.ToDateTime(paymentReader["PaymentDate"]),
                                    PaymentAmount = total,
                                    AmountReceived = amountReceived,
                                    ChangeAmount = changeAmount,
                                    PaymentMethod = method,
                                    PaymentStatus = "Paid",
                                    ProcessedBy = string.IsNullOrWhiteSpace(processedBy)
                                        ? "Front Desk" : processedBy.Trim()
                                };
                            }
                        }

                        using (SqlCommand update = new SqlCommand(
                            @"UPDATE dbo.Reservations
                              SET ReservationStatus = 'Checked Out'
                              WHERE ReservationId = @id
                                AND ReservationStatus = 'Checked In'", connection, transaction))
                        {
                            update.Parameters.Add("@id", SqlDbType.Int).Value = billing.ReservationId;
                            if (update.ExecuteNonQuery() != 1)
                                throw new InvalidOperationException(
                                    "Checkout could not update the reservation.");
                        }

                        transaction.Commit();
                        return receipt;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private const string BillingSelectSql = @"
            SELECT TOP 2
                   r.ReservationId,
                   g.GuestName,
                   g.ContactNumber,
                   g.Email,
                   g.HomeAddress,
                   g.IdType,
                   rm.RoomNumber,
                   rm.RoomType,
                   rm.RoomRate,
                   r.CheckInDate,
                   r.CheckOutDate,
                   r.NumberOfGuests,
                   r.Adults,
                   r.Children,
                   r.SpecialRequest
            FROM dbo.Reservations r
            INNER JOIN dbo.Guests g ON g.GuestId = r.GuestId
            INNER JOIN dbo.Rooms rm ON rm.RoomId = r.RoomId";

        private static BillingInfo ReadBilling(SqlDataReader reader)
        {
            DateTime arrival = Convert.ToDateTime(reader["CheckInDate"]);
            DateTime departure = Convert.ToDateTime(reader["CheckOutDate"]);
            decimal rate = Convert.ToDecimal(reader["RoomRate"]);
            int nights = Math.Max(1, (departure.Date - arrival.Date).Days);

            return new BillingInfo
            {
                ReservationId = Convert.ToInt32(reader["ReservationId"]),
                GuestName = Convert.ToString(reader["GuestName"]),
                GuestContactNumber = Convert.ToString(reader["ContactNumber"]),
                GuestEmail = Convert.ToString(reader["Email"]),
                GuestAddress = Convert.ToString(reader["HomeAddress"]),
                GuestIdType = Convert.ToString(reader["IdType"]),
                RoomNumber = Convert.ToString(reader["RoomNumber"]),
                RoomType = Convert.ToString(reader["RoomType"]),
                RoomRate = rate,
                CheckInDate = arrival,
                CheckOutDate = departure,
                StayDays = nights,
                NumberOfGuests = Convert.ToInt32(reader["NumberOfGuests"]),
                Adults = Convert.ToInt32(reader["Adults"]),
                Children = Convert.ToInt32(reader["Children"]),
                SpecialRequest = Convert.ToString(reader["SpecialRequest"]),
                RoomCharges = rate * nights,
                OtherCharges = 0m,
                TotalBill = rate * nights,
                ReservationStatus = "Checked In"
            };
        }
    }
}
