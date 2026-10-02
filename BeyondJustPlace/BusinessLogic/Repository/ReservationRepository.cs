using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Model;

namespace BusinessLogic.Repository
{
    public class ReservationRepository
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["HotelDB"].ConnectionString;

        public List<Guest> SearchGuest(string keyword)
        {
            List<Guest> guests = new List<Guest>();

            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                connection.Open();

                string query =
                    @"SELECT GuestId, GuestName, ContactNumber
                      FROM dbo.Guests
                      WHERE GuestName LIKE @keyword
                      OR ContactNumber LIKE @keyword
                      ORDER BY GuestName";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue(
                    "@keyword",
                    "%" + keyword + "%");

                SqlDataReader reader =
                    command.ExecuteReader();

                while (reader.Read())
                {
                    Guest guest = new Guest();

                    guest.GuestId =
                        Convert.ToInt32(reader["GuestId"]);

                    guest.GuestName =
                        reader["GuestName"].ToString();

                    guest.ContactNumber =
                        reader["ContactNumber"].ToString();

                    guests.Add(guest);
                }
            }

            return guests;
        }

        public int CreateGuest(
            string guestName,
            string contactNumber)
        {
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                connection.Open();

                string query =
                    @"INSERT INTO dbo.Guests
                      (GuestName, ContactNumber)
                      OUTPUT INSERTED.GuestId
                      VALUES
                      (@guestName, @contactNumber)";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue(
                    "@guestName",
                    guestName);

                command.Parameters.AddWithValue(
                    "@contactNumber",
                    contactNumber);

                return Convert.ToInt32(
                    command.ExecuteScalar());
            }
        }

        public List<string> GetRoomTypes()
        {
            List<string> types =
                new List<string>();

            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                connection.Open();

                string query =
                    @"SELECT DISTINCT RoomType
                      FROM dbo.Rooms
                      ORDER BY RoomType";

                SqlCommand command =
                    new SqlCommand(query, connection);

                SqlDataReader reader =
                    command.ExecuteReader();

                while (reader.Read())
                {
                    types.Add(
                        reader["RoomType"].ToString());
                }
            }

            return types;
        }

        public List<Room> GetAvailableRooms(
            DateTime checkIn,
            DateTime checkOut,
            string roomType)
        {
            List<Room> rooms =
                new List<Room>();

            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                connection.Open();

                string query =
                    @"SELECT
                        r.RoomId,
                        r.RoomNumber,
                        r.RoomType,
                        r.RoomRate,
                        r.RoomStatus
                      FROM dbo.Rooms r
                      WHERE r.RoomType = @roomType
                      AND r.RoomId NOT IN
                      (
                          SELECT RoomId
                          FROM dbo.Reservations
                          WHERE ReservationStatus IN
                          ('Pending', 'Confirmed', 'Checked In')
                          AND CheckInDate < @checkOut
                          AND CheckOutDate > @checkIn
                      )
                      ORDER BY r.RoomNumber";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue(
                    "@roomType",
                    roomType);

                command.Parameters.AddWithValue(
                    "@checkIn",
                    checkIn.Date);

                command.Parameters.AddWithValue(
                    "@checkOut",
                    checkOut.Date);

                SqlDataReader reader =
                    command.ExecuteReader();

                while (reader.Read())
                {
                    Room room = new Room();

                    room.RoomId =
                        Convert.ToInt32(
                            reader["RoomId"]);

                    room.RoomNumber =
                        reader["RoomNumber"].ToString();

                    room.RoomType =
                        reader["RoomType"].ToString();

                    room.RoomRate =
                        Convert.ToDecimal(
                            reader["RoomRate"]);

                    room.RoomStatus =
                        reader["RoomStatus"].ToString();

                    rooms.Add(room);
                }
            }

            return rooms;
        }

        public int CreateReservation(
            int guestId,
            int roomId,
            DateTime checkIn,
            DateTime checkOut,
            string status)
        {
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                connection.Open();

                string query =
                    @"INSERT INTO dbo.Reservations
                      (
                          GuestId,
                          RoomId,
                          CheckInDate,
                          CheckOutDate,
                          ReservationStatus
                      )
                      OUTPUT INSERTED.ReservationId
                      VALUES
                      (
                          @guestId,
                          @roomId,
                          @checkIn,
                          @checkOut,
                          @status
                      )";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue(
                    "@guestId",
                    guestId);

                command.Parameters.AddWithValue(
                    "@roomId",
                    roomId);

                command.Parameters.AddWithValue(
                    "@checkIn",
                    checkIn.Date);

                command.Parameters.AddWithValue(
                    "@checkOut",
                    checkOut.Date);

                command.Parameters.AddWithValue(
                    "@status",
                    status);

                return Convert.ToInt32(
                    command.ExecuteScalar());
            }
        }

        public int CancelReservation(int guestId)
        {
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                connection.Open();

                string query =
                    @"UPDATE dbo.Reservations
                      SET ReservationStatus = 'Cancelled'
                      WHERE ReservationId =
                      (
                          SELECT TOP 1 ReservationId
                          FROM dbo.Reservations
                          WHERE GuestId = @guestId
                          AND ReservationStatus IN
                          ('Pending', 'Confirmed')
                          ORDER BY ReservationId DESC
                      )";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue(
                    "@guestId",
                    guestId);

                return command.ExecuteNonQuery();
            }
        }

        public DataTable SearchCheckInReservations(
            string guestName)
        {
            DataTable table =
                new DataTable();

            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                connection.Open();

                string query =
                    @"SELECT
                        r.ReservationId,
                        g.GuestName,
                        rm.RoomNumber,
                        r.CheckInDate,
                        r.ReservationStatus
                      FROM dbo.Reservations r
                      INNER JOIN dbo.Guests g
                      ON r.GuestId = g.GuestId
                      INNER JOIN dbo.Rooms rm
                      ON r.RoomId = rm.RoomId
                      WHERE g.GuestName LIKE @guestName
                      AND r.ReservationStatus IN
                      ('Pending', 'Confirmed')
                      ORDER BY r.ReservationId DESC";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue(
                    "@guestName",
                    "%" + guestName + "%");

                SqlDataAdapter adapter =
                    new SqlDataAdapter(command);

                adapter.Fill(table);
            }

            return table;
        }

        public void CheckInGuest(int reservationId)
        {
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                connection.Open();

                string query =
                    @"UPDATE dbo.Reservations
                      SET ReservationStatus = 'Checked In'
                      WHERE ReservationId = @reservationId
                      AND ReservationStatus IN
                      ('Pending', 'Confirmed')";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue(
                    "@reservationId",
                    reservationId);

                command.ExecuteNonQuery();
            }
        }

        public int GetTodayReservationCount()
        {
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                connection.Open();

                string query =
                    @"SELECT COUNT(*)
                      FROM dbo.Reservations
                      WHERE CAST(CheckInDate AS DATE) =
                            CAST(GETDATE() AS DATE)
                      AND ReservationStatus <> 'Cancelled'";

                SqlCommand command =
                    new SqlCommand(query, connection);

                return Convert.ToInt32(
                    command.ExecuteScalar());
            }
        }

        public int GetCurrentCheckedInCount()
        {
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                connection.Open();

                string query =
                    @"SELECT COUNT(*)
                      FROM dbo.Reservations
                      WHERE ReservationStatus = 'Checked In'";

                SqlCommand command =
                    new SqlCommand(query, connection);

                return Convert.ToInt32(
                    command.ExecuteScalar());
            }
        }

        public int GetExpectedCheckInsTodayCount()
        {
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                connection.Open();

                string query =
                    @"SELECT COUNT(*)
                      FROM dbo.Reservations
                      WHERE CAST(CheckInDate AS DATE) =
                            CAST(GETDATE() AS DATE)
                      AND ReservationStatus IN
                      ('Pending', 'Confirmed')";

                SqlCommand command =
                    new SqlCommand(query, connection);

                return Convert.ToInt32(
                    command.ExecuteScalar());
            }
        }

        public int GetPendingReservationCount()
        {
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                connection.Open();

                string query =
                    @"SELECT COUNT(*)
                      FROM dbo.Reservations
                      WHERE ReservationStatus = 'Pending'";

                SqlCommand command =
                    new SqlCommand(query, connection);

                return Convert.ToInt32(
                    command.ExecuteScalar());
            }
        }

        public List<string> GetLatestActivity()
        {
            List<string> activities =
                new List<string>();

            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                connection.Open();

                string query =
                    @"SELECT TOP 10
                        r.ReservationId,
                        g.GuestName,
                        rm.RoomNumber,
                        r.ReservationStatus
                      FROM dbo.Reservations r
                      INNER JOIN dbo.Guests g
                      ON r.GuestId = g.GuestId
                      INNER JOIN dbo.Rooms rm
                      ON r.RoomId = rm.RoomId
                      ORDER BY r.ReservationId DESC";

                SqlCommand command =
                    new SqlCommand(query, connection);

                SqlDataReader reader =
                    command.ExecuteReader();

                while (reader.Read())
                {
                    string activity =
                        "Reservation #" +
                        reader["ReservationId"] +
                        " - " +
                        reader["GuestName"] +
                        " - Room " +
                        reader["RoomNumber"] +
                        " - " +
                        reader["ReservationStatus"];

                    activities.Add(activity);
                }
            }

            return activities;
        }
    }
}