using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
                @"SELECT r.ReservationId,
                         g.GuestName,
                         COALESCE(NULLIF(
                            STUFF((
                                SELECT N', ' + rm2.RoomNumber
                                FROM dbo.ReservationRooms rr2
                                INNER JOIN dbo.Rooms rm2 ON rm2.RoomId = rr2.RoomId
                                WHERE rr2.ReservationId = r.ReservationId
                                ORDER BY rm2.RoomNumber
                                FOR XML PATH(''), TYPE
                            ).value('.', 'nvarchar(max)'), 1, 2, N''), N''), rm.RoomNumber) AS RoomNumber,
                         r.CheckInDate,
                         r.ReservationStatus
                  FROM dbo.Reservations r
                  INNER JOIN dbo.Guests g ON g.GuestId = r.GuestId
                  INNER JOIN dbo.Rooms rm ON rm.RoomId = r.RoomId
                  WHERE g.GuestName LIKE @guestName
                    AND r.ReservationStatus IN ('Pending','Confirmed')
                  ORDER BY r.CheckInDate, r.ReservationId", connection))
            {
                command.Parameters.Add("@guestName", SqlDbType.NVarChar, 255).Value =
                    "%" + (keyword ?? "").Trim() + "%";

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
                    AND NOT EXISTS
                    (
                        SELECT 1
                        FROM dbo.Reservations r
                        WHERE r.ReservationStatus IN ('Pending','Confirmed','Checked In')
                          AND r.CheckInDate < @checkOut
                          AND r.CheckOutDate > @checkIn
                          AND
                          (
                              EXISTS
                              (
                                  SELECT 1
                                  FROM dbo.ReservationRooms rr
                                  WHERE rr.ReservationId = r.ReservationId
                                    AND rr.RoomId = rm.RoomId
                              )
                              OR
                              (
                                  NOT EXISTS
                                  (
                                      SELECT 1
                                      FROM dbo.ReservationRooms rr2
                                      WHERE rr2.ReservationId = r.ReservationId
                                  )
                                  AND r.RoomId = rm.RoomId
                              )
                          )
                    )
                  ORDER BY rm.RoomNumber", connection))
            {
                command.Parameters.Add("@roomType", SqlDbType.NVarChar, 100).Value =
                    (roomType ?? "").Trim();
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
            using (SqlCommand command = new SqlCommand(
                @"SELECT TOP 2 r.ReservationId
                  FROM dbo.Reservations r
                  INNER JOIN dbo.Guests g ON g.GuestId = r.GuestId
                  WHERE g.GuestName LIKE @guestName
                    AND r.ReservationStatus = 'Checked In'
                  ORDER BY r.ReservationId DESC", connection))
            {
                command.Parameters.Add("@guestName", SqlDbType.NVarChar, 255).Value =
                    "%" + (keyword ?? "").Trim() + "%";
                connection.Open();

                List<int> ids = new List<int>();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        ids.Add(Convert.ToInt32(reader["ReservationId"]));
                }

                if (ids.Count == 0) return null;
                if (ids.Count > 1)
                    throw new InvalidOperationException(
                        "More than one checked-in reservation matched. " +
                        "Search using the full guest name or reservation ID.");

                return LoadBilling(connection, ids[0]);
            }
        }

        public BillingInfo GetCheckedInBillingByReservationId(int reservationId)
        {
            if (reservationId <= 0)
                throw new InvalidOperationException("Enter a valid reservation ID.");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return LoadBilling(connection, reservationId);
            }
        }

        public void CompleteCheckout(BillingInfo billing, string method)
        {
            if (billing == null)
                throw new InvalidOperationException("Select a checked-in guest first.");

            CompleteCheckout(billing, method, billing.TotalBill, "Front Desk");
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
                throw new InvalidOperationException(
                    "Amount received must cover the total bill.");

            decimal changeAmount = method == "Cash"
                ? Math.Max(0m, amountReceived - billing.TotalBill)
                : 0m;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlTransaction transaction =
                    connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        decimal total;

                        using (SqlCommand read = new SqlCommand(
                            @"SELECT r.CheckInDate,
                                     r.CheckOutDate,
                                     COALESCE(
                                         (SELECT SUM(rr.RoomRateSnapshot)
                                          FROM dbo.ReservationRooms rr
                                          WHERE rr.ReservationId = r.ReservationId),
                                         rm.RoomRate) AS TotalNightlyRate
                              FROM dbo.Reservations r WITH (UPDLOCK, HOLDLOCK)
                              INNER JOIN dbo.Rooms rm ON rm.RoomId = r.RoomId
                              WHERE r.ReservationId = @id
                                AND r.ReservationStatus = 'Checked In'
                                AND NOT EXISTS
                                (
                                    SELECT 1
                                    FROM dbo.Payments p
                                    WHERE p.ReservationId = r.ReservationId
                                      AND p.PaymentStatus = 'Paid'
                                )", connection, transaction))
                        {
                            read.Parameters.Add("@id", SqlDbType.Int).Value = billing.ReservationId;

                            using (SqlDataReader reader = read.ExecuteReader())
                            {
                                if (!reader.Read())
                                    throw new InvalidOperationException(
                                        "This reservation is no longer checked in, or is already paid.");

                                decimal nightlyRate =
                                    Convert.ToDecimal(reader["TotalNightlyRate"]);
                                DateTime start = Convert.ToDateTime(reader["CheckInDate"]);
                                DateTime end = Convert.ToDateTime(reader["CheckOutDate"]);
                                total = nightlyRate * Math.Max(1, (end.Date - start.Date).Days);
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
                              VALUES
                              (@id, @amount, @method, 'Paid',
                               @received, @change, @processedBy)",
                            connection, transaction))
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

                            string staff = string.IsNullOrWhiteSpace(processedBy)
                                ? "Front Desk" : processedBy.Trim();

                            payment.Parameters.Add("@processedBy", SqlDbType.NVarChar, 100).Value = staff;

                            using (SqlDataReader paymentReader = payment.ExecuteReader())
                            {
                                if (!paymentReader.Read())
                                    throw new InvalidOperationException(
                                        "Payment could not be recorded.");

                                receipt = new PaymentReceiptInfo
                                {
                                    PaymentId = Convert.ToInt32(paymentReader["PaymentId"]),
                                    PaymentDate = Convert.ToDateTime(paymentReader["PaymentDate"]),
                                    PaymentAmount = total,
                                    AmountReceived = amountReceived,
                                    ChangeAmount = changeAmount,
                                    PaymentMethod = method,
                                    PaymentStatus = "Paid",
                                    ProcessedBy = staff
                                };
                            }
                        }

                        using (SqlCommand update = new SqlCommand(
                            @"UPDATE dbo.Reservations
                              SET ReservationStatus = 'Checked Out'
                              WHERE ReservationId = @id
                                AND ReservationStatus = 'Checked In'",
                            connection, transaction))
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
                        try { transaction.Rollback(); } catch { }
                        throw;
                    }
                }
            }
        }

        private static BillingInfo LoadBilling(SqlConnection connection, int reservationId)
        {
            int primaryRoomId;
            BillingInfo billing;

            using (SqlCommand command = new SqlCommand(
                @"SELECT r.ReservationId,
                         r.RoomId,
                         r.CheckInDate,
                         r.CheckOutDate,
                         r.NumberOfGuests,
                         r.Adults,
                         r.Children,
                         r.RoomCount,
                         r.SpecialRequest,
                         r.ReservationStatus,
                         g.GuestName,
                         g.ContactNumber,
                         g.Email,
                         g.HomeAddress,
                         g.IdType
                  FROM dbo.Reservations r
                  INNER JOIN dbo.Guests g ON g.GuestId = r.GuestId
                  WHERE r.ReservationId = @id
                    AND r.ReservationStatus = 'Checked In'", connection))
            {
                command.Parameters.Add("@id", SqlDbType.Int).Value = reservationId;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read()) return null;

                    DateTime arrival = Convert.ToDateTime(reader["CheckInDate"]);
                    DateTime departure = Convert.ToDateTime(reader["CheckOutDate"]);

                    billing = new BillingInfo
                    {
                        ReservationId = Convert.ToInt32(reader["ReservationId"]),
                        GuestName = Convert.ToString(reader["GuestName"]),
                        GuestContactNumber = Convert.ToString(reader["ContactNumber"]),
                        GuestEmail = Convert.ToString(reader["Email"]),
                        GuestAddress = Convert.ToString(reader["HomeAddress"]),
                        GuestIdType = Convert.ToString(reader["IdType"]),
                        CheckInDate = arrival,
                        CheckOutDate = departure,
                        StayDays = Math.Max(1, (departure.Date - arrival.Date).Days),
                        RoomCount = Convert.ToInt32(reader["RoomCount"]),
                        NumberOfGuests = Convert.ToInt32(reader["NumberOfGuests"]),
                        Adults = Convert.ToInt32(reader["Adults"]),
                        Children = Convert.ToInt32(reader["Children"]),
                        SpecialRequest = Convert.ToString(reader["SpecialRequest"]),
                        ReservationStatus = Convert.ToString(reader["ReservationStatus"])
                    };

                    primaryRoomId = Convert.ToInt32(reader["RoomId"]);
                }
            }

            List<string> roomNumbers = new List<string>();
            List<string> roomTypes = new List<string>();
            decimal totalNightlyRate = 0m;

            using (SqlCommand roomCommand = new SqlCommand(
                @"SELECT rm.RoomNumber,
                         rm.RoomType,
                         rr.RoomRateSnapshot
                  FROM dbo.ReservationRooms rr
                  INNER JOIN dbo.Rooms rm ON rm.RoomId = rr.RoomId
                  WHERE rr.ReservationId = @id
                  ORDER BY rm.RoomNumber", connection))
            {
                roomCommand.Parameters.Add("@id", SqlDbType.Int).Value = reservationId;

                using (SqlDataReader reader = roomCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        roomNumbers.Add(Convert.ToString(reader["RoomNumber"]));
                        string type = Convert.ToString(reader["RoomType"]);
                        if (!roomTypes.Contains(type)) roomTypes.Add(type);
                        totalNightlyRate += Convert.ToDecimal(reader["RoomRateSnapshot"]);
                    }
                }
            }

            // Backward compatibility for a reservation created before ReservationRooms existed.
            if (roomNumbers.Count == 0)
            {
                using (SqlCommand fallback = new SqlCommand(
                    @"SELECT RoomNumber, RoomType, RoomRate
                      FROM dbo.Rooms
                      WHERE RoomId = @roomId", connection))
                {
                    fallback.Parameters.Add("@roomId", SqlDbType.Int).Value = primaryRoomId;
                    using (SqlDataReader reader = fallback.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            roomNumbers.Add(Convert.ToString(reader["RoomNumber"]));
                            roomTypes.Add(Convert.ToString(reader["RoomType"]));
                            totalNightlyRate = Convert.ToDecimal(reader["RoomRate"]);
                        }
                    }
                }
            }

            billing.RoomCount = roomNumbers.Count;
            billing.RoomNumber = string.Join(", ", roomNumbers.ToArray());
            billing.RoomType = string.Join(", ", roomTypes.ToArray());
            billing.RoomRate = totalNightlyRate;
            billing.RoomCharges = totalNightlyRate * billing.StayDays;
            billing.OtherCharges = 0m;
            billing.TotalBill = billing.RoomCharges + billing.OtherCharges;

            return billing;
        }
    }
}
