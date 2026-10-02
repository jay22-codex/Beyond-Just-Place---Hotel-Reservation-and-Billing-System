using System;
using System.Configuration;
using System.Data.SqlClient;
using Model;

namespace BusinessLogic.Repository
{
    public class BillingRepository
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["HotelDB"].ConnectionString;

        public BillingInfo SearchGuest(string guestName)
        {
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                connection.Open();

                string query =
                    @"SELECT TOP 1
                        r.ReservationId,
                        g.GuestName,
                        rm.RoomNumber,
                        rm.RoomRate,
                        r.CheckInDate,
                        r.CheckOutDate,
                        r.ReservationStatus
                      FROM dbo.Reservations r
                      INNER JOIN dbo.Guests g
                      ON r.GuestId = g.GuestId
                      INNER JOIN dbo.Rooms rm
                      ON r.RoomId = rm.RoomId
                      WHERE g.GuestName LIKE @guestName
                      AND r.ReservationStatus = 'Checked In'
                      ORDER BY r.ReservationId DESC";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue(
                    "@guestName",
                    "%" + guestName + "%");

                SqlDataReader reader =
                    command.ExecuteReader();

                if (reader.Read())
                {
                    DateTime checkIn =
                        Convert.ToDateTime(
                            reader["CheckInDate"]);

                    DateTime checkOut =
                        Convert.ToDateTime(
                            reader["CheckOutDate"]);

                    int stayDays =
                        (checkOut.Date -
                         checkIn.Date).Days;

                    decimal roomRate =
                        Convert.ToDecimal(
                            reader["RoomRate"]);

                    decimal roomCharges =
                        roomRate * stayDays;

                    decimal otherCharges = 0;

                    BillingInfo billing =
                        new BillingInfo();

                    billing.ReservationId =
                        Convert.ToInt32(
                            reader["ReservationId"]);

                    billing.GuestName =
                        reader["GuestName"].ToString();

                    billing.RoomNumber =
                        reader["RoomNumber"].ToString();

                    billing.StayDays =
                        stayDays;

                    billing.RoomRate =
                        roomRate;

                    billing.RoomCharges =
                        roomCharges;

                    billing.OtherCharges =
                        otherCharges;

                    billing.TotalBill =
                        roomCharges +
                        otherCharges;

                    billing.ReservationStatus =
                        reader["ReservationStatus"].ToString();

                    return billing;
                }
            }

            return null;
        }

        public bool IsPaid(int reservationId)
        {
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                connection.Open();

                string query =
                    @"SELECT COUNT(*)
                      FROM dbo.Payments
                      WHERE ReservationId = @reservationId
                      AND PaymentStatus = 'Paid'";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue(
                    "@reservationId",
                    reservationId);

                int count =
                    Convert.ToInt32(
                        command.ExecuteScalar());

                return count > 0;
            }
        }

        public void SavePayment(
            int reservationId,
            decimal amount,
            string paymentMethod)
        {
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                connection.Open();

                string query =
                    @"INSERT INTO dbo.Payments
                      (
                          ReservationId,
                          PaymentAmount,
                          PaymentMethod,
                          PaymentStatus
                      )
                      VALUES
                      (
                          @reservationId,
                          @amount,
                          @paymentMethod,
                          'Paid'
                      )";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue(
                    "@reservationId",
                    reservationId);

                command.Parameters.AddWithValue(
                    "@amount",
                    amount);

                command.Parameters.AddWithValue(
                    "@paymentMethod",
                    paymentMethod);

                command.ExecuteNonQuery();

                string updateQuery =
                    @"UPDATE dbo.Reservations
                      SET ReservationStatus = 'Checked Out'
                      WHERE ReservationId = @reservationId";

                SqlCommand updateCommand =
                    new SqlCommand(
                        updateQuery,
                        connection);

                updateCommand.Parameters.AddWithValue(
                    "@reservationId",
                    reservationId);

                updateCommand.ExecuteNonQuery();
            }
        }
    }
}