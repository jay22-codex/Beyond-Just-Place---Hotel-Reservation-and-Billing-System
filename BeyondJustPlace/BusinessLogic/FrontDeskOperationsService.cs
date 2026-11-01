using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Model;

namespace BusinessLogic
{
    // Uses the SAME HotelDB connection as ReservationRepository and BillingRepository.
    public class FrontDeskOperationsService
    {
        private readonly string connectionString;

        public FrontDeskOperationsService()
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["HotelDB"];
            if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
                throw new InvalidOperationException("HotelDB connection string was not found in UI/App.config.");
            connectionString = settings.ConnectionString;
        }

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

        // RoomRate is already used in BillingRepository. RoomStatus is displayed as
        // availability computed from overlapping reservations, so no extra DB column is needed.
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
            using (SqlCommand command = new SqlCommand(
                @"SELECT TOP 2 r.ReservationId, g.GuestName, rm.RoomNumber,
                         rm.RoomRate, r.CheckInDate, r.CheckOutDate
                  FROM dbo.Reservations r
                  INNER JOIN dbo.Guests g ON g.GuestId = r.GuestId
                  INNER JOIN dbo.Rooms rm ON rm.RoomId = r.RoomId
                  WHERE g.GuestName LIKE @guestName
                    AND r.ReservationStatus = 'Checked In'
                  ORDER BY r.ReservationId DESC", connection))
            {
                command.Parameters.Add("@guestName", SqlDbType.NVarChar, 255).Value = "%" + (keyword ?? "").Trim() + "%";
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    BillingInfo billing = new BillingInfo();
                    billing.ReservationId = Convert.ToInt32(reader["ReservationId"]);
                    billing.GuestName = Convert.ToString(reader["GuestName"]);
                    billing.RoomNumber = Convert.ToString(reader["RoomNumber"]);
                    billing.RoomRate = Convert.ToDecimal(reader["RoomRate"]);
                    DateTime arrival = Convert.ToDateTime(reader["CheckInDate"]);
                    DateTime departure = Convert.ToDateTime(reader["CheckOutDate"]);
                    billing.StayDays = Math.Max(1, (departure.Date - arrival.Date).Days);
                    billing.RoomCharges = billing.StayDays * billing.RoomRate;
                    billing.OtherCharges = 0m;
                    billing.TotalBill = billing.RoomCharges;
                    billing.ReservationStatus = "Checked In";
                    if (reader.Read())
                        throw new InvalidOperationException("More than one checked-in reservation matched. Search using the full guest name, then resolve any duplicates.");
                    return billing;
                }
            }
        }

        // All-or-nothing payment and status update. Prevents duplicated payment on retry.
        public void CompleteCheckout(BillingInfo billing, string method)
        {
            if (billing == null) throw new InvalidOperationException("Select a checked-in guest first.");
            if (method != "Cash" && method != "GCash" && method != "Card")
                throw new InvalidOperationException("Select Cash, GCash, or Card.");

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
                                    throw new InvalidOperationException("This reservation is no longer checked in, or is already paid.");
                                decimal rate = Convert.ToDecimal(reader["RoomRate"]);
                                DateTime start = Convert.ToDateTime(reader["CheckInDate"]);
                                DateTime end = Convert.ToDateTime(reader["CheckOutDate"]);
                                total = rate * Math.Max(1, (end.Date - start.Date).Days);
                            }
                        }
                        if (total != billing.TotalBill)
                            throw new InvalidOperationException("The billing amount has changed. Search the guest again.");

                        using (SqlCommand payment = new SqlCommand(
                            @"INSERT INTO dbo.Payments
                              (ReservationId, PaymentAmount, PaymentMethod, PaymentStatus)
                              VALUES (@id, @amount, @method, 'Paid')", connection, transaction))
                        {
                            payment.Parameters.Add("@id", SqlDbType.Int).Value = billing.ReservationId;
                            SqlParameter amount = payment.Parameters.Add("@amount", SqlDbType.Decimal);
                            amount.Precision = 18;
                            amount.Scale = 2;
                            amount.Value = total;
                            payment.Parameters.Add("@method", SqlDbType.NVarChar, 30).Value = method;
                            payment.ExecuteNonQuery();
                        }
                        using (SqlCommand update = new SqlCommand(
                            @"UPDATE dbo.Reservations
                              SET ReservationStatus = 'Checked Out'
                              WHERE ReservationId = @id
                                AND ReservationStatus = 'Checked In'", connection, transaction))
                        {
                            update.Parameters.Add("@id", SqlDbType.Int).Value = billing.ReservationId;
                            if (update.ExecuteNonQuery() != 1)
                                throw new InvalidOperationException("Checkout could not update the reservation.");
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}