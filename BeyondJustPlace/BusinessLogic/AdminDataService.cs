using System;
using System.Data;
using System.Data.SqlClient;

namespace BusinessLogic
{
    public sealed class AdminDashboardSnapshot
    {
        public int TotalReservations { get; set; }
        public int CheckedIn { get; set; }
        public int PendingReservations { get; set; }
        public decimal TotalRevenue { get; set; }
        public DataTable LatestActivity { get; set; }
    }

    public class AdminDataService
    {
        private readonly string connectionString = DatabaseConfig.ConnectionString;

        public AdminDashboardSnapshot GetDashboardSnapshot()
        {
            AdminDashboardSnapshot snapshot = new AdminDashboardSnapshot();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                snapshot.TotalReservations = ScalarInt(connection,
                    "SELECT COUNT(*) FROM dbo.Reservations WHERE ReservationStatus <> N'Cancelled'");
                snapshot.CheckedIn = ScalarInt(connection,
                    "SELECT COUNT(*) FROM dbo.Reservations WHERE ReservationStatus = N'Checked In'");
                snapshot.PendingReservations = ScalarInt(connection,
                    "SELECT COUNT(*) FROM dbo.Reservations WHERE ReservationStatus = N'Pending'");
                snapshot.TotalRevenue = ScalarDecimal(connection,
                    @"SELECT ISNULL((SELECT SUM(PaymentAmount) FROM dbo.Payments WHERE PaymentStatus=N'Paid'),0)
                      + ISNULL((SELECT SUM(ReservationFeeAmount) FROM dbo.Reservations
                                WHERE ReservationFeeStatus=N'Paid'),0)");

                snapshot.LatestActivity = FillTable(connection, @"
SELECT TOP 15
    r.ReservationId AS [Reservation ID],
    g.GuestName AS [Guest],
    COALESCE(NULLIF(
        STUFF((
            SELECT N', ' + rm2.RoomNumber
            FROM dbo.ReservationRooms rr2
            INNER JOIN dbo.Rooms rm2 ON rm2.RoomId = rr2.RoomId
            WHERE rr2.ReservationId = r.ReservationId
            ORDER BY rm2.RoomNumber
            FOR XML PATH(''), TYPE
        ).value('.', 'nvarchar(max)'), 1, 2, N''), N''), rm.RoomNumber) AS [Room],
    r.ReservationStatus AS [Status]
FROM dbo.Reservations r
INNER JOIN dbo.Guests g ON g.GuestId = r.GuestId
INNER JOIN dbo.Rooms rm ON rm.RoomId = r.RoomId
ORDER BY r.ReservationId DESC");
            }

            return snapshot;
        }

        public DataTable GetRooms()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return FillTable(connection, @"
SELECT RoomId, RoomNumber, RoomType, RoomRate, RoomCapacity,
       BedCount, SizeSqm, Description, IsActive, ReservationFee, RoomStatus
FROM dbo.Rooms
ORDER BY RoomNumber");
            }
        }

        public int AddRoom(string roomNumber, string roomType, decimal rate,
            int capacity, decimal reservationFee, string status)
        {
            ValidateRoom(roomNumber, roomType, rate, capacity, reservationFee, status);
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(@"
INSERT INTO dbo.Rooms
(RoomNumber, RoomType, RoomRate, RoomCapacity, ReservationFee, RoomStatus)
OUTPUT INSERTED.RoomId
VALUES (@number, @type, @rate, @capacity, @fee, @status)", connection))
            {
                AddRoomParameters(command, roomNumber, roomType, rate,
                    capacity, reservationFee, status);
                connection.Open();
                using(SqlCommand dup=new SqlCommand("SELECT COUNT(*) FROM dbo.Rooms WHERE RoomNumber=@number",connection))
                {
                    dup.Parameters.Add("@number",SqlDbType.NVarChar,20).Value=roomNumber.Trim();
                    if(Convert.ToInt32(dup.ExecuteScalar())>0)
                        throw new InvalidOperationException("Duplicate room number. Use another room number.");
                }
                using(SqlCommand typeCheck=new SqlCommand("SELECT COUNT(*) FROM dbo.RoomTypes WHERE TypeName=@type AND IsActive=1",connection))
                {
                    typeCheck.Parameters.Add("@type",SqlDbType.NVarChar,100).Value=roomType.Trim();
                    if(Convert.ToInt32(typeCheck.ExecuteScalar())==0)throw new InvalidOperationException("Select an active room type from Admin Room Type Setup.");
                }
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void UpdateRoom(int roomId, string roomNumber, string roomType,
            decimal rate, int capacity, decimal reservationFee, string status)
        {
            if (roomId <= 0) throw new InvalidOperationException("Select a room first.");
            ValidateRoom(roomNumber, roomType, rate, capacity, reservationFee, status);

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(@"
UPDATE dbo.Rooms
SET RoomNumber=@number, RoomType=@type, RoomRate=@rate,
    RoomCapacity=@capacity, ReservationFee=@fee, RoomStatus=@status
WHERE RoomId=@id", connection))
            {
                AddRoomParameters(command, roomNumber, roomType, rate,
                    capacity, reservationFee, status);
                command.Parameters.Add("@id", SqlDbType.Int).Value = roomId;
                connection.Open();
                using(SqlCommand dup=new SqlCommand("SELECT COUNT(*) FROM dbo.Rooms WHERE RoomNumber=@number AND RoomId<>@id",connection))
                {
                    dup.Parameters.Add("@number",SqlDbType.NVarChar,20).Value=roomNumber.Trim();
                    dup.Parameters.Add("@id",SqlDbType.Int).Value=roomId;
                    if(Convert.ToInt32(dup.ExecuteScalar())>0)
                        throw new InvalidOperationException("Duplicate room number. Use another room number.");
                }
                using(SqlCommand typeCheck=new SqlCommand("SELECT COUNT(*) FROM dbo.RoomTypes WHERE TypeName=@type AND IsActive=1",connection))
                {
                    typeCheck.Parameters.Add("@type",SqlDbType.NVarChar,100).Value=roomType.Trim();
                    if(Convert.ToInt32(typeCheck.ExecuteScalar())==0)throw new InvalidOperationException("Select an active room type from Admin Room Type Setup.");
                }
                if (command.ExecuteNonQuery() != 1)
                    throw new InvalidOperationException("Room was not found.");
            }
        }

        // Legacy API: deactivate rather than deleting reservation history.
        public void DeleteRoom(int roomId)
        {
            new HotelEnhancementService().SetRoomActive(roomId, false);
        }

        public DataTable GetUsers()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return FillTable(connection,
                    "SELECT UserId, Username, Role, IsActive FROM dbo.Users ORDER BY Username");
            }
        }

        public void AddUser(string username, string password, string role)
        {
            ValidateUser(username, password, role, true);
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(@"
INSERT INTO dbo.Users(Username, Password, Role)
VALUES(@username, @password, @role)", connection))
            {
                command.Parameters.Add("@username", SqlDbType.NVarChar, 50).Value = username.Trim();
                command.Parameters.Add("@password", SqlDbType.NVarChar, 300).Value = PasswordSecurity.Hash(password);
                command.Parameters.Add("@role", SqlDbType.NVarChar, 20).Value = NormalizeRole(role);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void UpdateUser(int userId, string username, string newPassword, string role)
        {
            if (userId <= 0) throw new InvalidOperationException("Select a user first.");
            ValidateUser(username, newPassword, role, false);
            string sql = string.IsNullOrWhiteSpace(newPassword)
                ? "UPDATE dbo.Users SET Username=@username, Role=@role WHERE UserId=@id"
                : "UPDATE dbo.Users SET Username=@username, Password=@password, Role=@role WHERE UserId=@id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@username", SqlDbType.NVarChar, 50).Value = username.Trim();
                command.Parameters.Add("@role", SqlDbType.NVarChar, 20).Value = NormalizeRole(role);
                command.Parameters.Add("@id", SqlDbType.Int).Value = userId;
                if (!string.IsNullOrWhiteSpace(newPassword))
                    command.Parameters.Add("@password", SqlDbType.NVarChar, 300).Value = PasswordSecurity.Hash(newPassword);
                connection.Open();
                using(SqlCommand check=new SqlCommand("SELECT Role FROM dbo.Users WHERE UserId=@id",connection))
                {
                    check.Parameters.Add("@id",SqlDbType.Int).Value=userId;
                    string previous=Convert.ToString(check.ExecuteScalar());
                    if(previous=="Admin" && NormalizeRole(role)!="Admin")
                    {
                        using(SqlCommand count=new SqlCommand("SELECT COUNT(*) FROM dbo.Users WHERE Role='Admin' AND IsActive=1",connection))
                            if(Convert.ToInt32(count.ExecuteScalar())<2)throw new InvalidOperationException("At least one active Admin account must remain.");
                    }
                }
                if (command.ExecuteNonQuery() != 1)
                    throw new InvalidOperationException("User was not found.");
            }
        }

        // Permanent account deletion is intentionally disabled.
        // Admin must deactivate an account using the User Maintenance screen,
        // which checks the current user and ensures an Admin remains active.
        public void DeleteUser(int userId)
        {
            throw new InvalidOperationException(
                "Permanent deletion is disabled. Use Activate / Deactivate in User Maintenance.");
        }

        public DataTable GetReservations()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return FillTable(connection, @"
SELECT r.ReservationId,
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
       r.RoomCount,
       r.CheckInDate, r.CheckOutDate, r.ReservationStatus,
       r.NumberOfGuests, r.Adults, r.Children, r.SpecialRequest,
       r.ReservationFeeAmount,r.ReservationFeePaymentMethod,r.ReservationFeeStatus,r.ReservationFeeReference
FROM dbo.Reservations r
INNER JOIN dbo.Guests g ON g.GuestId=r.GuestId
INNER JOIN dbo.Rooms rm ON rm.RoomId=r.RoomId
ORDER BY r.ReservationId DESC");
            }
        }

        public DataTable GetPayments()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return FillTable(connection, @"
SELECT p.PaymentId, p.ReservationId, g.GuestName,
       COALESCE(NULLIF(
          STUFF((
              SELECT N', ' + rm2.RoomNumber
              FROM dbo.ReservationRooms rr2
              INNER JOIN dbo.Rooms rm2 ON rm2.RoomId = rr2.RoomId
              WHERE rr2.ReservationId = r.ReservationId
              ORDER BY rm2.RoomNumber
              FOR XML PATH(''), TYPE
          ).value('.', 'nvarchar(max)'), 1, 2, N''), N''), rm.RoomNumber) AS RoomNumber,
       p.PaymentAmount, p.PaymentMethod, p.PaymentReference,
       p.PaymentStatus, p.PaymentDate
FROM dbo.Payments p
INNER JOIN dbo.Reservations r ON r.ReservationId=p.ReservationId
INNER JOIN dbo.Guests g ON g.GuestId=r.GuestId
INNER JOIN dbo.Rooms rm ON rm.RoomId=r.RoomId
ORDER BY p.PaymentId DESC");
            }
        }

        private static int ScalarInt(SqlConnection connection, string sql)
        {
            using (SqlCommand command = new SqlCommand(sql, connection))
                return Convert.ToInt32(command.ExecuteScalar());
        }

        private static decimal ScalarDecimal(SqlConnection connection, string sql)
        {
            using (SqlCommand command = new SqlCommand(sql, connection))
                return Convert.ToDecimal(command.ExecuteScalar());
        }

        private static DataTable FillTable(SqlConnection connection, string sql)
        {
            using (SqlDataAdapter adapter = new SqlDataAdapter(sql, connection))
            {
                DataTable table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        private static void ValidateRoom(string number, string type, decimal rate,
            int capacity, decimal fee, string status)
        {
            if (string.IsNullOrWhiteSpace(number)) throw new InvalidOperationException("Room number is required.");
            if (string.IsNullOrWhiteSpace(type)) throw new InvalidOperationException("Room type is required.");
            if (rate < 0) throw new InvalidOperationException("Room rate cannot be negative.");
            if (capacity < 1) throw new InvalidOperationException("Room capacity must be at least 1.");
            if (fee < 0) throw new InvalidOperationException("Reservation fee cannot be negative.");
            if (status != "Available" && status != "Maintenance" && status != "Unavailable")
                throw new InvalidOperationException("Choose Available, Maintenance, or Unavailable.");
        }

        private static void AddRoomParameters(SqlCommand command, string number,
            string type, decimal rate, int capacity, decimal fee, string status)
        {
            command.Parameters.Add("@number", SqlDbType.NVarChar, 20).Value = number.Trim();
            command.Parameters.Add("@type", SqlDbType.NVarChar, 100).Value = type.Trim();
            SqlParameter rateParam = command.Parameters.Add("@rate", SqlDbType.Decimal);
            rateParam.Precision = 18; rateParam.Scale = 2; rateParam.Value = rate;
            command.Parameters.Add("@capacity", SqlDbType.Int).Value = capacity;
            SqlParameter feeParam = command.Parameters.Add("@fee", SqlDbType.Decimal);
            feeParam.Precision = 18; feeParam.Scale = 2; feeParam.Value = fee;
            command.Parameters.Add("@status", SqlDbType.NVarChar, 30).Value = status;
        }

        private static void ValidateUser(string username, string password,
            string role, bool passwordRequired)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new InvalidOperationException("Username is required.");
            if (username.Trim().Length > 50) throw new InvalidOperationException("Username is too long.");
            if (passwordRequired && string.IsNullOrWhiteSpace(password)) throw new InvalidOperationException("Password is required.");
            if (!string.IsNullOrEmpty(password) && password.Length > 100) throw new InvalidOperationException("Password is too long.");
            NormalizeRole(role);
        }

        private static string NormalizeRole(string role)
        {
            string normalized = (role ?? "").Replace(" ", "").Trim();
            if (normalized.Equals("Admin", StringComparison.OrdinalIgnoreCase)) return "Admin";
            if (normalized.Equals("FrontDesk", StringComparison.OrdinalIgnoreCase)) return "FrontDesk";
            throw new InvalidOperationException("Role must be Admin or FrontDesk.");
        }
    }
}
