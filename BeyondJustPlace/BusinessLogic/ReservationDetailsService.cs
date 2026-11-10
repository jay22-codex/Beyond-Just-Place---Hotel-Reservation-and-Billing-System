
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Model;

namespace BusinessLogic
{
    public class AvailableRoomInfo
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string RoomType { get; set; }
        public decimal RoomRate { get; set; }
        public int? RoomCapacity { get; set; }
        public decimal? ReservationFee { get; set; }
    }

    public class ReservationDetailsService
    {
        private readonly string connectionString = DatabaseConfig.ConnectionString;

        public ReservationDetailsService()
        {
        }

        public Guest GetGuest(int guestId)
        {
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT GuestId, GuestName, ContactNumber,
                         Email, HomeAddress, IdType
                  FROM dbo.Guests
                  WHERE GuestId = @id", connection))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value =
                    guestId;

                connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    return new Guest
                    {
                        GuestId = Convert.ToInt32(reader["GuestId"]),
                        GuestName = Convert.ToString(reader["GuestName"]),
                        ContactNumber =
                            Convert.ToString(reader["ContactNumber"]),
                        Email = Convert.ToString(reader["Email"]),
                        HomeAddress =
                            Convert.ToString(reader["HomeAddress"]),
                        IdType = Convert.ToString(reader["IdType"])
                    };
                }
            }
        }

        public Guest SaveGuest(Guest guest)
        {
            if (guest == null ||
                string.IsNullOrWhiteSpace(guest.GuestName) ||
                string.IsNullOrWhiteSpace(guest.ContactNumber))
                throw new InvalidOperationException(
                    "Guest name and contact number are required.");

            if (guest.GuestName.Trim().Length > 100 ||
                guest.ContactNumber.Trim().Length > 20 ||
                (guest.Email ?? "").Length > 254 ||
                (guest.HomeAddress ?? "").Length > 500 ||
                (guest.IdType ?? "").Length > 50)
                throw new InvalidOperationException(
                    "One of the guest fields is too long.");

            string sql = guest.GuestId == 0
                ? @"INSERT INTO dbo.Guests
                    (GuestName, ContactNumber, Email,
                     HomeAddress, IdType)
                    OUTPUT INSERTED.GuestId
                    VALUES
                    (@name, @contact, @email, @address, @idType)"
                : @"UPDATE dbo.Guests
                    SET GuestName = @name,
                        ContactNumber = @contact,
                        Email = @email,
                        HomeAddress = @address,
                        IdType = @idType
                    WHERE GuestId = @id";

            using (SqlConnection connection =
                new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, connection))
            {
                cmd.Parameters.Add("@name", SqlDbType.NVarChar, 255)
                    .Value = guest.GuestName.Trim();

                cmd.Parameters.Add("@contact", SqlDbType.NVarChar, 50)
                    .Value = guest.ContactNumber.Trim();

                cmd.Parameters.Add("@email", SqlDbType.NVarChar, 254)
                    .Value = NullIfEmpty(guest.Email);

                cmd.Parameters.Add("@address", SqlDbType.NVarChar, 500)
                    .Value = NullIfEmpty(guest.HomeAddress);

                cmd.Parameters.Add("@idType", SqlDbType.NVarChar, 50)
                    .Value = NullIfEmpty(guest.IdType);

                connection.Open();

                if (guest.GuestId == 0)
                {
                    guest.GuestId =
                        Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int)
                        .Value = guest.GuestId;

                    if (cmd.ExecuteNonQuery() != 1)
                        throw new InvalidOperationException(
                            "Guest record not found. Search again.");
                }
            }

            return guest;
        }

        public List<AvailableRoomInfo> GetAvailableRooms(
            DateTime checkIn,
            DateTime checkOut,
            string roomType)
        {
            if (checkOut.Date <= checkIn.Date)
                throw new InvalidOperationException(
                    "Check-out date must be after check-in date.");

            if (string.IsNullOrWhiteSpace(roomType))
                throw new InvalidOperationException(
                    "Select a room type.");

            List<AvailableRoomInfo> rooms =
                new List<AvailableRoomInfo>();

            using (SqlConnection connection =
                new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT
                      rm.RoomId,
                      rm.RoomNumber,
                      rm.RoomType,
                      rm.RoomRate,
                      rm.RoomCapacity,
                      rm.ReservationFee
                  FROM dbo.Rooms rm
                  WHERE rm.RoomType = @type
                    AND ISNULL(rm.RoomStatus, N'Available') = N'Available'
                    AND NOT EXISTS
                    (
                        SELECT 1
                        FROM dbo.Reservations r
                        WHERE r.RoomId = rm.RoomId
                          AND r.ReservationStatus IN
                              ('Pending', 'Confirmed', 'Checked In')
                          AND r.CheckInDate < @checkOut
                          AND r.CheckOutDate > @checkIn
                    )
                  ORDER BY rm.RoomNumber", connection))
            {
                cmd.Parameters.Add("@type", SqlDbType.NVarChar, 100)
                    .Value = roomType;

                cmd.Parameters.Add("@checkIn", SqlDbType.Date)
                    .Value = checkIn.Date;

                cmd.Parameters.Add("@checkOut", SqlDbType.Date)
                    .Value = checkOut.Date;

                connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rooms.Add(new AvailableRoomInfo
                        {
                            RoomId =
                                Convert.ToInt32(reader["RoomId"]),

                            RoomNumber =
                                Convert.ToString(reader["RoomNumber"]),

                            RoomType =
                                Convert.ToString(reader["RoomType"]),

                            RoomRate =
                                Convert.ToDecimal(reader["RoomRate"]),

                            RoomCapacity =
                                reader["RoomCapacity"] == DBNull.Value
                                ? (int?)null
                                : Convert.ToInt32(reader["RoomCapacity"]),

                            ReservationFee =
                                reader["ReservationFee"] == DBNull.Value
                                ? (decimal?)null
                                : Convert.ToDecimal(reader["ReservationFee"])
                        });
                    }
                }
            }

            return rooms;
        }

        public int CreateReservation(
            int guestId,
            AvailableRoomInfo room,
            DateTime checkIn,
            DateTime checkOut,
            int guests,
            int adults,
            int children,
            int roomCount,
            string request,
            string feePaymentMethod)
        {
            if (guestId <= 0 || room == null)
                throw new InvalidOperationException(
                    "Save/select a guest and select a room first.");

            if (roomCount != 1)
                throw new InvalidOperationException(
                    "Only one room per reservation is supported currently. " +
                    "Select Rooms = 1.");

            if (checkOut.Date <= checkIn.Date)
                throw new InvalidOperationException(
                    "Check-out must be after check-in.");

            if (guests <= 0 || adults < 1 ||
                children < 0 || adults + children != guests)
                throw new InvalidOperationException(
                    "Number of Guests must equal Adults + Children " +
                    "(at least one adult).");

            if (!string.IsNullOrWhiteSpace(request) &&
                request.Length > 1000)
                throw new InvalidOperationException(
                    "Special requests must be 1000 characters or fewer.");

            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlTransaction transaction =
                    connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        int? capacity;
                        decimal? fee;
                        decimal currentRate;

                        using (SqlCommand read = new SqlCommand(
                            @"SELECT RoomCapacity, ReservationFee, RoomRate
                              FROM dbo.Rooms WITH (UPDLOCK, HOLDLOCK)
                              WHERE RoomId = @roomId",
                            connection, transaction))
                        {
                            read.Parameters.Add("@roomId", SqlDbType.Int)
                                .Value = room.RoomId;

                            using (SqlDataReader reader =
                                read.ExecuteReader())
                            {
                                if (!reader.Read())
                                    throw new InvalidOperationException(
                                        "Selected room no longer exists.");

                                capacity =
                                    reader["RoomCapacity"] == DBNull.Value
                                    ? (int?)null
                                    : Convert.ToInt32(
                                        reader["RoomCapacity"]);

                                fee =
                                    reader["ReservationFee"] == DBNull.Value
                                    ? (decimal?)null
                                    : Convert.ToDecimal(
                                        reader["ReservationFee"]);

                                currentRate =
                                    Convert.ToDecimal(reader["RoomRate"]);
                            }
                        }

                        if (currentRate != room.RoomRate)
                            throw new InvalidOperationException(
                                "The room rate changed. " +
                                "Check availability again.");

                        if (capacity.HasValue &&
                            guests > capacity.Value)
                            throw new InvalidOperationException(
                                "Guest count exceeds room capacity of " +
                                capacity.Value + ".");

                        using (SqlCommand conflict = new SqlCommand(
                            @"SELECT COUNT(*)
                              FROM dbo.Reservations
                                   WITH (UPDLOCK, HOLDLOCK)
                              WHERE RoomId = @roomId
                                AND ReservationStatus IN
                                    ('Pending', 'Confirmed', 'Checked In')
                                AND CheckInDate < @checkOut
                                AND CheckOutDate > @checkIn",
                            connection, transaction))
                        {
                            conflict.Parameters.Add(
                                "@roomId", SqlDbType.Int)
                                .Value = room.RoomId;

                            conflict.Parameters.Add(
                                "@checkIn", SqlDbType.Date)
                                .Value = checkIn.Date;

                            conflict.Parameters.Add(
                                "@checkOut", SqlDbType.Date)
                                .Value = checkOut.Date;

                            if (Convert.ToInt32(
                                conflict.ExecuteScalar()) > 0)
                                throw new InvalidOperationException(
                                    "This room has just been reserved. " +
                                    "Check availability again.");
                        }

                        int newId;

                        using (SqlCommand insert = new SqlCommand(
                            @"INSERT INTO dbo.Reservations
                              (
                                  GuestId, RoomId,
                                  CheckInDate, CheckOutDate,
                                  ReservationStatus,
                                  NumberOfGuests, Adults, Children,
                                  RoomCount, SpecialRequest,
                                  ReservationFeeAmount,
                                  ReservationFeePaymentMethod
                              )
                              OUTPUT INSERTED.ReservationId
                              VALUES
                              (
                                  @guestId, @roomId,
                                  @checkIn, @checkOut,
                                  'Pending',
                                  @guests, @adults, @children,
                                  @roomCount, @request,
                                  @fee, @feeMethod
                              )",
                            connection, transaction))
                        {
                            insert.Parameters.Add(
                                "@guestId", SqlDbType.Int)
                                .Value = guestId;

                            insert.Parameters.Add(
                                "@roomId", SqlDbType.Int)
                                .Value = room.RoomId;

                            insert.Parameters.Add(
                                "@checkIn", SqlDbType.Date)
                                .Value = checkIn.Date;

                            insert.Parameters.Add(
                                "@checkOut", SqlDbType.Date)
                                .Value = checkOut.Date;

                            insert.Parameters.Add(
                                "@guests", SqlDbType.Int)
                                .Value = guests;

                            insert.Parameters.Add(
                                "@adults", SqlDbType.Int)
                                .Value = adults;

                            insert.Parameters.Add(
                                "@children", SqlDbType.Int)
                                .Value = children;

                            insert.Parameters.Add(
                                "@roomCount", SqlDbType.Int)
                                .Value = roomCount;

                            insert.Parameters.Add(
                                "@request", SqlDbType.NVarChar, 1000)
                                .Value = NullIfEmpty(request);

                            SqlParameter amount =
                                insert.Parameters.Add(
                                    "@fee", SqlDbType.Decimal);

                            amount.Precision = 18;
                            amount.Scale = 2;
                            amount.Value = fee.HasValue
                                ? (object)fee.Value
                                : DBNull.Value;

                            insert.Parameters.Add(
                                "@feeMethod", SqlDbType.NVarChar, 30)
                                .Value = NullIfEmpty(feePaymentMethod);

                            newId = Convert.ToInt32(
                                insert.ExecuteScalar());
                        }

                        transaction.Commit();
                        return newId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private static object NullIfEmpty(string text)
        {
            return string.IsNullOrWhiteSpace(text)
                ? (object)DBNull.Value
                : text.Trim();
        }
    }
}
