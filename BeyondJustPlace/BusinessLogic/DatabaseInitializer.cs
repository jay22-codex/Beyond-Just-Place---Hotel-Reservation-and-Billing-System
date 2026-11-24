using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;

namespace BusinessLogic
{
    /// <summary>
    /// Creates and upgrades the local database automatically at application startup.
    /// Safe to run every time the program starts.
    /// </summary>
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            EnsureLocalDbIsReachable();
            EnsureDatabaseExists();
            EnsureSchema();
            SeedDefaults();
        }

        private static void EnsureLocalDbIsReachable()
        {
            Exception lastError = null;

            for (int attempt = 0; attempt < 3; attempt++)
            {
                try
                {
                    using (SqlConnection connection =
                        new SqlConnection(DatabaseConfig.MasterConnectionString))
                    {
                        connection.Open();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    lastError = ex;

                    if (attempt == 0)
                        TryStartAutomaticLocalDbInstance();

                    Thread.Sleep(1200);
                }
            }

            throw new InvalidOperationException(
                "SQL Server Express LocalDB could not be started. " +
                "Install the SQL Server Express LocalDB component through " +
                "Visual Studio Installer, then run the program again.\n\n" +
                "Technical details: " +
                (lastError == null ? "Unknown LocalDB error." : lastError.Message),
                lastError);
        }

        private static void TryStartAutomaticLocalDbInstance()
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = "SqlLocalDB.exe";
                info.Arguments = "start MSSQLLocalDB";
                info.CreateNoWindow = true;
                info.UseShellExecute = false;
                info.RedirectStandardOutput = true;
                info.RedirectStandardError = true;

                using (Process process = Process.Start(info))
                {
                    if (process != null)
                        process.WaitForExit(10000);
                }
            }
            catch
            {
                // Opening the automatic LocalDB instance normally starts it.
                // If SqlLocalDB.exe is not on PATH, the later connection error
                // gives the user the installation instruction.
            }
        }

        private static void EnsureDatabaseExists()
        {
            using (SqlConnection connection =
                new SqlConnection(DatabaseConfig.MasterConnectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText =
                    "IF DB_ID(N'" + DatabaseConfig.DatabaseName + "') IS NULL " +
                    "BEGIN CREATE DATABASE [" + DatabaseConfig.DatabaseName + "]; END;";
                command.CommandTimeout = 60;
                command.ExecuteNonQuery();
            }
        }

        private static void EnsureSchema()
        {
            using (SqlConnection connection =
                new SqlConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();

                Execute(connection, @"
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        UserId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
        Username NVARCHAR(50) NOT NULL,
        Password NVARCHAR(100) NOT NULL,
        Role NVARCHAR(20) NOT NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT SYSDATETIME()
    );
END;

IF OBJECT_ID(N'dbo.Guests', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Guests
    (
        GuestId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Guests PRIMARY KEY,
        GuestName NVARCHAR(255) NOT NULL,
        ContactNumber NVARCHAR(50) NOT NULL,
        Email NVARCHAR(254) NULL,
        HomeAddress NVARCHAR(500) NULL,
        IdType NVARCHAR(50) NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Guests_CreatedAt DEFAULT SYSDATETIME()
    );
END;

IF OBJECT_ID(N'dbo.Rooms', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Rooms
    (
        RoomId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Rooms PRIMARY KEY,
        RoomNumber NVARCHAR(20) NOT NULL,
        RoomType NVARCHAR(100) NOT NULL,
        RoomRate DECIMAL(18,2) NOT NULL,
        RoomStatus NVARCHAR(30) NOT NULL CONSTRAINT DF_Rooms_Status DEFAULT N'Available',
        RoomCapacity INT NULL,
        ReservationFee DECIMAL(18,2) NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Rooms_CreatedAt DEFAULT SYSDATETIME()
    );
END;

IF OBJECT_ID(N'dbo.Reservations', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Reservations
    (
        ReservationId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Reservations PRIMARY KEY,
        GuestId INT NOT NULL,
        RoomId INT NOT NULL,
        CheckInDate DATE NOT NULL,
        CheckOutDate DATE NOT NULL,
        ReservationStatus NVARCHAR(30) NOT NULL,
        NumberOfGuests INT NOT NULL CONSTRAINT DF_Reservations_Guests DEFAULT 1,
        Adults INT NOT NULL CONSTRAINT DF_Reservations_Adults DEFAULT 1,
        Children INT NOT NULL CONSTRAINT DF_Reservations_Children DEFAULT 0,
        RoomCount INT NOT NULL CONSTRAINT DF_Reservations_RoomCount DEFAULT 1,
        SpecialRequest NVARCHAR(1000) NULL,
        ReservationFeeAmount DECIMAL(18,2) NULL,
        ReservationFeePaymentMethod NVARCHAR(30) NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Reservations_CreatedAt DEFAULT SYSDATETIME(),
        CONSTRAINT FK_Reservations_Guests FOREIGN KEY (GuestId) REFERENCES dbo.Guests(GuestId),
        CONSTRAINT FK_Reservations_Rooms FOREIGN KEY (RoomId) REFERENCES dbo.Rooms(RoomId),
        CONSTRAINT CK_Reservations_Dates CHECK (CheckOutDate > CheckInDate)
    );
END;

IF OBJECT_ID(N'dbo.ReservationRooms', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ReservationRooms
    (
        ReservationRoomId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ReservationRooms PRIMARY KEY,
        ReservationId INT NOT NULL,
        RoomId INT NOT NULL,
        RoomRateSnapshot DECIMAL(18,2) NOT NULL,
        ReservationFeeSnapshot DECIMAL(18,2) NULL,
        CONSTRAINT FK_ReservationRooms_Reservations FOREIGN KEY (ReservationId)
            REFERENCES dbo.Reservations(ReservationId),
        CONSTRAINT FK_ReservationRooms_Rooms FOREIGN KEY (RoomId)
            REFERENCES dbo.Rooms(RoomId)
    );
END;

IF OBJECT_ID(N'dbo.Payments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Payments
    (
        PaymentId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Payments PRIMARY KEY,
        ReservationId INT NOT NULL,
        PaymentAmount DECIMAL(18,2) NOT NULL,
        PaymentMethod NVARCHAR(30) NOT NULL,
        PaymentStatus NVARCHAR(30) NOT NULL,
        AmountReceived DECIMAL(18,2) NULL,
        ChangeAmount DECIMAL(18,2) NULL,
        ProcessedBy NVARCHAR(100) NULL,
        PaymentDate DATETIME2 NOT NULL CONSTRAINT DF_Payments_Date DEFAULT SYSDATETIME(),
        CONSTRAINT FK_Payments_Reservations FOREIGN KEY (ReservationId)
            REFERENCES dbo.Reservations(ReservationId)
    );
END;");

                // Upgrade older local databases without deleting their records.
                EnsureColumn(connection, "dbo.Guests", "Email", "NVARCHAR(254) NULL");
                EnsureColumn(connection, "dbo.Guests", "HomeAddress", "NVARCHAR(500) NULL");
                EnsureColumn(connection, "dbo.Guests", "IdType", "NVARCHAR(50) NULL");

                EnsureColumn(connection, "dbo.Rooms", "RoomStatus", "NVARCHAR(30) NOT NULL CONSTRAINT DF_Rooms_Status_Upgrade DEFAULT N'Available'");
                EnsureColumn(connection, "dbo.Rooms", "RoomCapacity", "INT NULL");
                EnsureColumn(connection, "dbo.Rooms", "ReservationFee", "DECIMAL(18,2) NULL");

                EnsureColumn(connection, "dbo.Reservations", "NumberOfGuests", "INT NOT NULL CONSTRAINT DF_Reservations_Guests_Upgrade DEFAULT 1");
                EnsureColumn(connection, "dbo.Reservations", "Adults", "INT NOT NULL CONSTRAINT DF_Reservations_Adults_Upgrade DEFAULT 1");
                EnsureColumn(connection, "dbo.Reservations", "Children", "INT NOT NULL CONSTRAINT DF_Reservations_Children_Upgrade DEFAULT 0");
                EnsureColumn(connection, "dbo.Reservations", "RoomCount", "INT NOT NULL CONSTRAINT DF_Reservations_RoomCount_Upgrade DEFAULT 1");
                EnsureColumn(connection, "dbo.Reservations", "SpecialRequest", "NVARCHAR(1000) NULL");
                EnsureColumn(connection, "dbo.Reservations", "ReservationFeeAmount", "DECIMAL(18,2) NULL");
                EnsureColumn(connection, "dbo.Reservations", "ReservationFeePaymentMethod", "NVARCHAR(30) NULL");
                EnsureColumn(connection, "dbo.Reservations", "CreatedAt", "DATETIME2 NOT NULL CONSTRAINT DF_Reservations_CreatedAt_Upgrade DEFAULT SYSDATETIME()");

                // Safe upgrades for professor-required guest, user and room data.
                EnsureColumn(connection, "dbo.Guests", "IdNumber", "NVARCHAR(100) NULL");
                EnsureColumn(connection, "dbo.Guests", "IsActive", "BIT NOT NULL CONSTRAINT DF_Guests_IsActive DEFAULT 1");
                EnsureColumn(connection, "dbo.Guests", "IdVerified", "BIT NOT NULL CONSTRAINT DF_Guests_IdVerified DEFAULT 0");
                EnsureColumn(connection, "dbo.Users", "IsActive", "BIT NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT 1");
                Execute(connection, @"IF COL_LENGTH(N'dbo.Users',N'Password') < 600
                    ALTER TABLE dbo.Users ALTER COLUMN [Password] NVARCHAR(300) NOT NULL;");
                EnsureColumn(connection, "dbo.Rooms", "BedCount", "INT NULL");
                EnsureColumn(connection, "dbo.Rooms", "SizeSqm", "INT NULL");
                EnsureColumn(connection, "dbo.Rooms", "Description", "NVARCHAR(500) NULL");
                EnsureColumn(connection, "dbo.Rooms", "IsActive", "BIT NOT NULL CONSTRAINT DF_Rooms_IsActive DEFAULT 1");
                EnsureColumn(connection, "dbo.Reservations", "ReservationFeeStatus", "NVARCHAR(30) NOT NULL CONSTRAINT DF_Reservations_FeeStatus DEFAULT N'Unpaid'");
                EnsureColumn(connection, "dbo.Reservations", "ReservationFeeReference", "NVARCHAR(100) NULL");
                EnsureColumn(connection, "dbo.Reservations", "KeysIssued", "BIT NOT NULL CONSTRAINT DF_Reservations_KeysIssued DEFAULT 0");
                EnsureColumn(connection, "dbo.Reservations", "IdentityChecked", "BIT NOT NULL CONSTRAINT DF_Reservations_IdChecked DEFAULT 0");
                EnsureColumn(connection, "dbo.Payments", "PaymentReference", "NVARCHAR(100) NULL");

                Execute(connection, @"
IF OBJECT_ID(N'dbo.RoomTypes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RoomTypes
    (
        RoomTypeId INT IDENTITY(1,1) PRIMARY KEY,
        TypeName NVARCHAR(100) NOT NULL,
        Rate DECIMAL(18,2) NOT NULL,
        Capacity INT NOT NULL,
        BedCount INT NOT NULL,
        SizeSqm INT NOT NULL,
        ReservationFee DECIMAL(18,2) NOT NULL,
        Description NVARCHAR(500) NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1
    );
END;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'UX_RoomTypes_Name' AND object_id=OBJECT_ID(N'dbo.RoomTypes'))
    CREATE UNIQUE INDEX UX_RoomTypes_Name ON dbo.RoomTypes(TypeName);

INSERT INTO dbo.RoomTypes(TypeName, Rate, Capacity, BedCount, SizeSqm, ReservationFee, Description)
SELECT rm.RoomType, MAX(rm.RoomRate),
       CASE WHEN MAX(ISNULL(rm.RoomCapacity,0)) < 1 THEN 2 ELSE MAX(rm.RoomCapacity) END,
       CASE WHEN rm.RoomType=N'Family' THEN 3 WHEN rm.RoomType=N'Suite' THEN 2 ELSE 1 END,
       CASE WHEN rm.RoomType=N'Standard' THEN 24 WHEN rm.RoomType=N'Deluxe' THEN 32
            WHEN rm.RoomType=N'Family' THEN 50 ELSE 60 END,
       MAX(ISNULL(rm.ReservationFee,0)),
       rm.RoomType + N' hotel room with standard amenities.'
FROM dbo.Rooms rm
WHERE NOT EXISTS (SELECT 1 FROM dbo.RoomTypes rt WHERE rt.TypeName=rm.RoomType)
GROUP BY rm.RoomType;

UPDATE rm SET
  rm.BedCount=COALESCE(rm.BedCount,rt.BedCount),
  rm.SizeSqm=COALESCE(rm.SizeSqm,rt.SizeSqm),
  rm.Description=COALESCE(NULLIF(rm.Description,N''),rt.Description),
  rm.RoomCapacity=COALESCE(rm.RoomCapacity,rt.Capacity)
FROM dbo.Rooms rm INNER JOIN dbo.RoomTypes rt ON rt.TypeName=rm.RoomType;

IF OBJECT_ID(N'dbo.ReservationServices',N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ReservationServices
    (
        ServiceId INT IDENTITY(1,1) PRIMARY KEY,
        ReservationId INT NOT NULL REFERENCES dbo.Reservations(ReservationId),
        ServiceName NVARCHAR(30) NOT NULL,
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(18,2) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        CONSTRAINT CK_ReservationServices_Quantity CHECK(Quantity>0),
        CONSTRAINT CK_ReservationServices_Price CHECK(UnitPrice>=0)
    );
END;

IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name=N'UX_Reservations_GCashFeeRef' AND object_id=OBJECT_ID(N'dbo.Reservations'))
    CREATE UNIQUE INDEX UX_Reservations_GCashFeeRef ON dbo.Reservations(ReservationFeeReference)
    WHERE ReservationFeeReference IS NOT NULL;
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name=N'UX_Payments_GCashRef' AND object_id=OBJECT_ID(N'dbo.Payments'))
    CREATE UNIQUE INDEX UX_Payments_GCashRef ON dbo.Payments(PaymentReference)
    WHERE PaymentReference IS NOT NULL;
");

                // ReservationRooms is the multi-room child table. Backfill all
                // existing one-room reservations so old local databases upgrade safely.
                Execute(connection, @"
IF OBJECT_ID(N'dbo.ReservationRooms', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ReservationRooms
    (
        ReservationRoomId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ReservationRooms PRIMARY KEY,
        ReservationId INT NOT NULL,
        RoomId INT NOT NULL,
        RoomRateSnapshot DECIMAL(18,2) NOT NULL,
        ReservationFeeSnapshot DECIMAL(18,2) NULL,
        CONSTRAINT FK_ReservationRooms_Reservations FOREIGN KEY (ReservationId)
            REFERENCES dbo.Reservations(ReservationId),
        CONSTRAINT FK_ReservationRooms_Rooms FOREIGN KEY (RoomId)
            REFERENCES dbo.Rooms(RoomId)
    );
END;

INSERT INTO dbo.ReservationRooms
(ReservationId, RoomId, RoomRateSnapshot, ReservationFeeSnapshot)
SELECT r.ReservationId, r.RoomId, rm.RoomRate, rm.ReservationFee
FROM dbo.Reservations r
INNER JOIN dbo.Rooms rm ON rm.RoomId = r.RoomId
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.ReservationRooms rr
    WHERE rr.ReservationId = r.ReservationId
);");

                EnsureColumn(connection, "dbo.Payments", "AmountReceived", "DECIMAL(18,2) NULL");
                EnsureColumn(connection, "dbo.Payments", "ChangeAmount", "DECIMAL(18,2) NULL");
                EnsureColumn(connection, "dbo.Payments", "ProcessedBy", "NVARCHAR(100) NULL");
                EnsureColumn(connection, "dbo.Payments", "PaymentDate", "DATETIME2 NOT NULL CONSTRAINT DF_Payments_Date_Upgrade DEFAULT SYSDATETIME()");

                Execute(connection, @"
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Users_Username' AND object_id = OBJECT_ID(N'dbo.Users'))
   AND NOT EXISTS (SELECT Username FROM dbo.Users GROUP BY Username HAVING COUNT(*) > 1)
    CREATE UNIQUE INDEX UX_Users_Username ON dbo.Users(Username);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Rooms_RoomNumber' AND object_id = OBJECT_ID(N'dbo.Rooms'))
   AND NOT EXISTS (SELECT RoomNumber FROM dbo.Rooms GROUP BY RoomNumber HAVING COUNT(*) > 1)
    CREATE UNIQUE INDEX UX_Rooms_RoomNumber ON dbo.Rooms(RoomNumber);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Reservations_RoomDates' AND object_id = OBJECT_ID(N'dbo.Reservations'))
    CREATE INDEX IX_Reservations_RoomDates
        ON dbo.Reservations(RoomId, CheckInDate, CheckOutDate, ReservationStatus);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Reservations_GuestStatus' AND object_id = OBJECT_ID(N'dbo.Reservations'))
    CREATE INDEX IX_Reservations_GuestStatus
        ON dbo.Reservations(GuestId, ReservationStatus, ReservationId);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_ReservationRooms_ReservationRoom' AND object_id = OBJECT_ID(N'dbo.ReservationRooms'))
    CREATE UNIQUE INDEX UX_ReservationRooms_ReservationRoom
        ON dbo.ReservationRooms(ReservationId, RoomId);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ReservationRooms_RoomReservation' AND object_id = OBJECT_ID(N'dbo.ReservationRooms'))
    CREATE INDEX IX_ReservationRooms_RoomReservation
        ON dbo.ReservationRooms(RoomId, ReservationId);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Payments_PaidReservation' AND object_id = OBJECT_ID(N'dbo.Payments'))
   AND NOT EXISTS (SELECT ReservationId FROM dbo.Payments WHERE PaymentStatus=N'Paid' GROUP BY ReservationId HAVING COUNT(*) > 1)
    CREATE UNIQUE INDEX UX_Payments_PaidReservation
        ON dbo.Payments(ReservationId)
        WHERE PaymentStatus = N'Paid';");
            }
        }

        private static void EnsureColumn(
            SqlConnection connection,
            string tableName,
            string columnName,
            string definition)
        {
            string sql =
                "IF COL_LENGTH(N'" + tableName + "', N'" + columnName + "') IS NULL " +
                "ALTER TABLE " + tableName + " ADD [" + columnName + "] " + definition + ";";
            Execute(connection, sql);
        }

        private static void SeedDefaults()
        {
            using (SqlConnection connection =
                new SqlConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();

                Execute(connection, @"
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = N'admin')
    INSERT INTO dbo.Users (Username, Password, Role)
    VALUES (N'admin', N'admin123', N'Admin');

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = N'frontdesk')
    INSERT INTO dbo.Users (Username, Password, Role)
    VALUES (N'frontdesk', N'frontdesk123', N'FrontDesk');");

                using (SqlCommand countCommand = new SqlCommand(
                    "SELECT COUNT(*) FROM dbo.Rooms", connection))
                {
                    int roomCount = Convert.ToInt32(countCommand.ExecuteScalar());
                    if (roomCount == 0)
                    {
                        Execute(connection, @"
INSERT INTO dbo.Rooms
(RoomNumber, RoomType, RoomRate, RoomStatus, RoomCapacity, ReservationFee)
VALUES
(N'101', N'Standard', 1500.00, N'Available', 2, 500.00),
(N'102', N'Standard', 1500.00, N'Available', 2, 500.00),
(N'103', N'Standard', 1500.00, N'Available', 2, 500.00),
(N'201', N'Deluxe',   2300.00, N'Available', 3, 700.00),
(N'202', N'Deluxe',   2300.00, N'Available', 3, 700.00),
(N'203', N'Deluxe',   2300.00, N'Available', 3, 700.00),
(N'301', N'Family',   3200.00, N'Available', 5, 1000.00),
(N'302', N'Family',   3200.00, N'Available', 5, 1000.00),
(N'401', N'Suite',    4500.00, N'Available', 4, 1500.00),
(N'402', N'Suite',    4500.00, N'Available', 4, 1500.00);");
                    }
                }

                // Seed the type catalogue only after starter rooms are present.
                Execute(connection, @"
INSERT INTO dbo.RoomTypes(TypeName,Rate,Capacity,BedCount,SizeSqm,ReservationFee,Description)
SELECT src.TypeName,src.Rate,src.Capacity,src.Beds,src.SizeSqm,src.Fee,src.Description
FROM (VALUES
(N'Standard',1500.00,2,1,24,500.00,N'Comfortable standard room with one bed, television and work desk.'),
(N'Deluxe',2300.00,3,2,32,700.00,N'Spacious deluxe room with lounge seating and upgraded amenities.'),
(N'Family',3200.00,5,3,50,1000.00,N'Family room with multiple beds and extra living space.'),
(N'Suite',4500.00,4,2,60,1500.00,N'Suite with a separate lounge and premium amenities.')
) src(TypeName,Rate,Capacity,Beds,SizeSqm,Fee,Description)
WHERE NOT EXISTS (SELECT 1 FROM dbo.RoomTypes t WHERE t.TypeName=src.TypeName);
UPDATE rm SET rm.BedCount=COALESCE(rm.BedCount,t.BedCount),
    rm.SizeSqm=COALESCE(rm.SizeSqm,t.SizeSqm),
    rm.Description=COALESCE(rm.Description,t.Description)
FROM dbo.Rooms rm INNER JOIN dbo.RoomTypes t ON t.TypeName=rm.RoomType;
");

                // Maintain exactly at least 20 numbered records per default type.
                // Existing rooms and their reservations are never deleted or renumbered.
                // The seed is repeatable when teammates launch on their own LocalDB.
                Execute(connection, @"
DECLARE @roomType NVARCHAR(100), @prefix NVARCHAR(4);
DECLARE @rate DECIMAL(18,2), @capacity INT, @beds INT, @sqm INT, @fee DECIMAL(18,2);
DECLARE @description NVARCHAR(500);
DECLARE room_types_to_seed CURSOR LOCAL FAST_FORWARD FOR
SELECT t.TypeName,
       CASE t.TypeName
           WHEN N'Standard' THEN N'11'
           WHEN N'Deluxe'   THEN N'21'
           WHEN N'Family'   THEN N'31'
           WHEN N'Suite'    THEN N'41' END,
       t.Rate, t.Capacity, t.BedCount, t.SizeSqm, t.ReservationFee, t.Description
FROM dbo.RoomTypes t
WHERE t.TypeName IN (N'Standard',N'Deluxe',N'Family',N'Suite');
OPEN room_types_to_seed;
FETCH NEXT FROM room_types_to_seed
  INTO @roomType,@prefix,@rate,@capacity,@beds,@sqm,@fee,@description;
WHILE @@FETCH_STATUS = 0
BEGIN
    DECLARE @current INT = (SELECT COUNT(*) FROM dbo.Rooms WHERE RoomType=@roomType);
    DECLARE @seq INT = 1;
    WHILE @current < 20 AND @seq < 900
    BEGIN
        DECLARE @roomNumber NVARCHAR(20) = @prefix + RIGHT(N'00' + CONVERT(NVARCHAR(10),@seq),3);
        IF NOT EXISTS (SELECT 1 FROM dbo.Rooms WHERE RoomNumber=@roomNumber)
        BEGIN
            INSERT INTO dbo.Rooms
                (RoomNumber,RoomType,RoomRate,RoomStatus,RoomCapacity,
                 ReservationFee,BedCount,SizeSqm,Description,IsActive)
            VALUES
                (@roomNumber,@roomType,@rate,N'Available',@capacity,
                 @fee,@beds,@sqm,@description,1);
            SET @current = @current + 1;
        END;
        SET @seq = @seq + 1;
    END;
    FETCH NEXT FROM room_types_to_seed
      INTO @roomType,@prefix,@rate,@capacity,@beds,@sqm,@fee,@description;
END;
CLOSE room_types_to_seed;
DEALLOCATE room_types_to_seed;
");
            }
        }

        private static void Execute(SqlConnection connection, string sql)
        {
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 60;
                command.ExecuteNonQuery();
            }
        }
    }
}
