using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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

        public Guest GetGuest(int guestId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT GuestId, GuestName, ContactNumber,
                         Email, HomeAddress, IdType
                  FROM dbo.Guests
                  WHERE GuestId = @id", connection))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = guestId;
                connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;

                    return new Guest
                    {
                        GuestId = Convert.ToInt32(reader["GuestId"]),
                        GuestName = Convert.ToString(reader["GuestName"]),
                        ContactNumber = Convert.ToString(reader["ContactNumber"]),
                        Email = Convert.ToString(reader["Email"]),
                        HomeAddress = Convert.ToString(reader["HomeAddress"]),
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

            if (guest.GuestName.Trim().Length > 255 ||
                guest.ContactNumber.Trim().Length > 50 ||
                (guest.Email ?? "").Length > 254 ||
                (guest.HomeAddress ?? "").Length > 500 ||
                (guest.IdType ?? "").Length > 50)
                throw new InvalidOperationException(
                    "One of the guest fields is too long.");

            string sql = guest.GuestId == 0
                ? @"INSERT INTO dbo.Guests
                    (GuestName, ContactNumber, Email, HomeAddress, IdType)
                    OUTPUT INSERTED.GuestId
                    VALUES (@name, @contact, @email, @address, @idType)"
                : @"UPDATE dbo.Guests
                    SET GuestName = @name,
                        ContactNumber = @contact,
                        Email = @email,
                        HomeAddress = @address,
                        IdType = @idType
                    WHERE GuestId = @id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, connection))
            {
                cmd.Parameters.Add("@name", SqlDbType.NVarChar, 255).Value = guest.GuestName.Trim();
                cmd.Parameters.Add("@contact", SqlDbType.NVarChar, 50).Value = guest.ContactNumber.Trim();
                cmd.Parameters.Add("@email", SqlDbType.NVarChar, 254).Value = NullIfEmpty(guest.Email);
                cmd.Parameters.Add("@address", SqlDbType.NVarChar, 500).Value = NullIfEmpty(guest.HomeAddress);
                cmd.Parameters.Add("@idType", SqlDbType.NVarChar, 50).Value = NullIfEmpty(guest.IdType);

                connection.Open();

                if (guest.GuestId == 0)
                {
                    guest.GuestId = Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = guest.GuestId;
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
                throw new InvalidOperationException("Select a room type.");

            List<AvailableRoomInfo> rooms = new List<AvailableRoomInfo>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT rm.RoomId, rm.RoomNumber, rm.RoomType,
                         rm.RoomRate, rm.RoomCapacity, rm.ReservationFee
                  FROM dbo.Rooms rm
                  WHERE rm.RoomType = @type
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
                cmd.Parameters.Add("@type", SqlDbType.NVarChar, 100).Value = roomType.Trim();
                cmd.Parameters.Add("@checkIn", SqlDbType.Date).Value = checkIn.Date;
                cmd.Parameters.Add("@checkOut", SqlDbType.Date).Value = checkOut.Date;

                connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rooms.Add(new AvailableRoomInfo
                        {
                            RoomId = Convert.ToInt32(reader["RoomId"]),
                            RoomNumber = Convert.ToString(reader["RoomNumber"]),
                            RoomType = Convert.ToString(reader["RoomType"]),
                            RoomRate = Convert.ToDecimal(reader["RoomRate"]),
                            RoomCapacity = reader["RoomCapacity"] == DBNull.Value
                                ? (int?)null : Convert.ToInt32(reader["RoomCapacity"]),
                            ReservationFee = reader["ReservationFee"] == DBNull.Value
                                ? (decimal?)null : Convert.ToDecimal(reader["ReservationFee"])
                        });
                    }
                }
            }

            return rooms;
        }

        // Compatibility overload for older code that still passes one room.
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
            List<AvailableRoomInfo> rooms = new List<AvailableRoomInfo>();
            if (room != null) rooms.Add(room);

            return CreateReservation(
                guestId, rooms, checkIn, checkOut,
                guests, adults, children, roomCount,
                request, feePaymentMethod);
        }

        // One reservation can now contain one or many actual rooms.
        public int CreateReservation(
            int guestId,
            IList<AvailableRoomInfo> requestedRooms,
            DateTime checkIn,
            DateTime checkOut,
            int guests,
            int adults,
            int children,
            int roomCount,
            string request,
            string feePaymentMethod)
        {
            if (guestId <= 0)
                throw new InvalidOperationException("Save/select a guest first.");

            if (requestedRooms == null || requestedRooms.Count == 0)
                throw new InvalidOperationException("Select at least one available room.");

            if (roomCount < 1 || requestedRooms.Count != roomCount)
                throw new InvalidOperationException(
                    "Select exactly " + roomCount + " room(s) for this reservation.");

            if (requestedRooms.Select(r => r.RoomId).Distinct().Count() != requestedRooms.Count)
                throw new InvalidOperationException("The same room cannot be selected twice.");

            if (checkOut.Date <= checkIn.Date)
                throw new InvalidOperationException("Check-out must be after check-in.");

            if (guests <= 0 || adults < 1 || children < 0 || adults + children != guests)
                throw new InvalidOperationException(
                    "Number of Guests must equal Adults + Children (at least one adult).");

            if (!string.IsNullOrWhiteSpace(request) && request.Length > 1000)
                throw new InvalidOperationException(
                    "Special requests must be 1000 characters or fewer.");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlTransaction transaction =
                    connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        List<AvailableRoomInfo> confirmedRooms =
                            new List<AvailableRoomInfo>();

                        foreach (AvailableRoomInfo requested in requestedRooms)
                        {
                            AvailableRoomInfo current = LoadAndLockRoom(
                                connection, transaction, requested.RoomId);

                            if (current == null)
                                throw new InvalidOperationException(
                                    "One of the selected rooms no longer exists.");

                            if (current.RoomRate != requested.RoomRate)
                                throw new InvalidOperationException(
                                    "The rate for Room " + current.RoomNumber +
                                    " changed. Check availability again.");

                            if (RoomHasConflict(connection, transaction,
                                current.RoomId, checkIn, checkOut))
                                throw new InvalidOperationException(
                                    "Room " + current.RoomNumber +
                                    " has just been reserved. Check availability again.");

                            confirmedRooms.Add(current);
                        }

                        bool allCapacitiesKnown =
                            confirmedRooms.All(r => r.RoomCapacity.HasValue);

                        if (allCapacitiesKnown)
                        {
                            int totalCapacity = confirmedRooms.Sum(
                                r => r.RoomCapacity.GetValueOrDefault());

                            if (guests > totalCapacity)
                                throw new InvalidOperationException(
                                    "The selected rooms can hold only " + totalCapacity +
                                    " guest(s). Add more rooms or choose larger rooms.");
                        }

                        decimal totalReservationFee = confirmedRooms.Sum(
                            r => r.ReservationFee.GetValueOrDefault());

                        int newId;
                        int primaryRoomId = confirmedRooms[0].RoomId;

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
                                  N'Pending',
                                  @guests, @adults, @children,
                                  @roomCount, @request,
                                  @fee, @feeMethod
                              )", connection, transaction))
                        {
                            insert.Parameters.Add("@guestId", SqlDbType.Int).Value = guestId;
                            insert.Parameters.Add("@roomId", SqlDbType.Int).Value = primaryRoomId;
                            insert.Parameters.Add("@checkIn", SqlDbType.Date).Value = checkIn.Date;
                            insert.Parameters.Add("@checkOut", SqlDbType.Date).Value = checkOut.Date;
                            insert.Parameters.Add("@guests", SqlDbType.Int).Value = guests;
                            insert.Parameters.Add("@adults", SqlDbType.Int).Value = adults;
                            insert.Parameters.Add("@children", SqlDbType.Int).Value = children;
                            insert.Parameters.Add("@roomCount", SqlDbType.Int).Value = roomCount;
                            insert.Parameters.Add("@request", SqlDbType.NVarChar, 1000).Value = NullIfEmpty(request);

                            SqlParameter fee = insert.Parameters.Add("@fee", SqlDbType.Decimal);
                            fee.Precision = 18;
                            fee.Scale = 2;
                            fee.Value = totalReservationFee;

                            insert.Parameters.Add("@feeMethod", SqlDbType.NVarChar, 30).Value =
                                NullIfEmpty(feePaymentMethod);

                            newId = Convert.ToInt32(insert.ExecuteScalar());
                        }

                        foreach (AvailableRoomInfo room in confirmedRooms)
                        {
                            using (SqlCommand roomInsert = new SqlCommand(
                                @"INSERT INTO dbo.ReservationRooms
                                  (ReservationId, RoomId, RoomRateSnapshot, ReservationFeeSnapshot)
                                  VALUES
                                  (@reservationId, @roomId, @rate, @fee)",
                                connection, transaction))
                            {
                                roomInsert.Parameters.Add("@reservationId", SqlDbType.Int).Value = newId;
                                roomInsert.Parameters.Add("@roomId", SqlDbType.Int).Value = room.RoomId;

                                SqlParameter rate = roomInsert.Parameters.Add("@rate", SqlDbType.Decimal);
                                rate.Precision = 18;
                                rate.Scale = 2;
                                rate.Value = room.RoomRate;

                                SqlParameter roomFee = roomInsert.Parameters.Add("@fee", SqlDbType.Decimal);
                                roomFee.Precision = 18;
                                roomFee.Scale = 2;
                                roomFee.Value = room.ReservationFee.HasValue
                                    ? (object)room.ReservationFee.Value : DBNull.Value;

                                roomInsert.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return newId;
                    }
                    catch
                    {
                        try { transaction.Rollback(); } catch { }
                        throw;
                    }
                }
            }
        }

        private static AvailableRoomInfo LoadAndLockRoom(
            SqlConnection connection,
            SqlTransaction transaction,
            int roomId)
        {
            using (SqlCommand read = new SqlCommand(
                @"SELECT RoomId, RoomNumber, RoomType, RoomRate,
                         RoomCapacity, ReservationFee, RoomStatus
                  FROM dbo.Rooms WITH (UPDLOCK, HOLDLOCK)
                  WHERE RoomId = @roomId", connection, transaction))
            {
                read.Parameters.Add("@roomId", SqlDbType.Int).Value = roomId;

                using (SqlDataReader reader = read.ExecuteReader())
                {
                    if (!reader.Read()) return null;

                    string status = Convert.ToString(reader["RoomStatus"]);
                    if (!string.Equals(status, "Available",
                        StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException(
                            "Room " + Convert.ToString(reader["RoomNumber"]) +
                            " is not currently available.");

                    return new AvailableRoomInfo
                    {
                        RoomId = Convert.ToInt32(reader["RoomId"]),
                        RoomNumber = Convert.ToString(reader["RoomNumber"]),
                        RoomType = Convert.ToString(reader["RoomType"]),
                        RoomRate = Convert.ToDecimal(reader["RoomRate"]),
                        RoomCapacity = reader["RoomCapacity"] == DBNull.Value
                            ? (int?)null : Convert.ToInt32(reader["RoomCapacity"]),
                        ReservationFee = reader["ReservationFee"] == DBNull.Value
                            ? (decimal?)null : Convert.ToDecimal(reader["ReservationFee"])
                    };
                }
            }
        }

        private static bool RoomHasConflict(
            SqlConnection connection,
            SqlTransaction transaction,
            int roomId,
            DateTime checkIn,
            DateTime checkOut)
        {
            using (SqlCommand conflict = new SqlCommand(
                @"SELECT COUNT(*)
                  FROM dbo.Reservations r WITH (UPDLOCK, HOLDLOCK)
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
                              AND rr.RoomId = @roomId
                        )
                        OR
                        (
                            NOT EXISTS
                            (
                                SELECT 1
                                FROM dbo.ReservationRooms rr2
                                WHERE rr2.ReservationId = r.ReservationId
                            )
                            AND r.RoomId = @roomId
                        )
                    )", connection, transaction))
            {
                conflict.Parameters.Add("@roomId", SqlDbType.Int).Value = roomId;
                conflict.Parameters.Add("@checkIn", SqlDbType.Date).Value = checkIn.Date;
                conflict.Parameters.Add("@checkOut", SqlDbType.Date).Value = checkOut.Date;
                return Convert.ToInt32(conflict.ExecuteScalar()) > 0;
            }
        }

        private static object NullIfEmpty(string text)
        {
            return string.IsNullOrWhiteSpace(text)
                ? (object)DBNull.Value : text.Trim();
        }
    }
}
