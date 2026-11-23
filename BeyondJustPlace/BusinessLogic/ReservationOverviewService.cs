using System;
using System.Data;
using System.Data.SqlClient;

namespace BusinessLogic
{
    // Read-only overview used by Front Desk dashboards and reservation review.
    public sealed class ReservationOverviewService
    {
        private readonly string connectionString = DatabaseConfig.ConnectionString;

        public DataTable GetReservations(string keyword, bool checkInOnly = false)
        {
            DataTable result = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP (250)
    r.ReservationId AS ReservationId,
    r.GuestId AS GuestId,
    g.GuestName AS GuestName,
    g.ContactNumber AS ContactNumber,
    ISNULL(g.Email, N'') AS Email,
    ISNULL(g.HomeAddress, N'') AS HomeAddress,
    ISNULL(g.IdType, N'') AS IdType,
    ISNULL(g.IdNumber, N'') AS IdNumber,
    g.IdVerified AS IdVerified,
    r.CheckInDate AS CheckInDate,
    r.CheckOutDate AS CheckOutDate,
    r.NumberOfGuests AS NumberOfGuests,
    r.Adults AS Adults,
    r.Children AS Children,
    r.RoomCount AS RoomCount,
    ISNULL((SELECT SUM(ISNULL(rm2.BedCount,0)) FROM dbo.ReservationRooms rrr
      JOIN dbo.Rooms rm2 ON rm2.RoomId=rrr.RoomId WHERE rrr.ReservationId=r.ReservationId),rm.BedCount) AS BedCount,
    r.ReservationStatus AS ReservationStatus,
    ISNULL(r.ReservationFeeStatus, N'Unpaid') AS DepositStatus,
    ISNULL(r.ReservationFeeAmount, 0) AS DepositAmount,
    ISNULL(r.ReservationFeePaymentMethod, N'') AS ReservationFeePaymentMethod,
    ISNULL(r.SpecialRequest, N'') AS SpecialRequest,
    r.IdentityChecked AS IdentityChecked,
    r.KeysIssued AS KeysIssued,
    COALESCE(NULLIF(STUFF((
        SELECT N', ' + r2.RoomNumber
        FROM dbo.ReservationRooms rr
        JOIN dbo.Rooms r2 ON r2.RoomId = rr.RoomId
        WHERE rr.ReservationId = r.ReservationId
        ORDER BY r2.RoomNumber
        FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'),1,2,N''),N''),
        rm.RoomNumber) AS RoomNumbers
FROM dbo.Reservations r
JOIN dbo.Guests g ON g.GuestId=r.GuestId
JOIN dbo.Rooms rm ON rm.RoomId=r.RoomId
WHERE (@checkin=0 OR r.ReservationStatus IN (N'Pending',N'Confirmed'))
  AND (@keyword=N'' OR g.GuestName LIKE @pattern OR g.ContactNumber LIKE @pattern
       OR CONVERT(NVARCHAR(20),r.ReservationId)=@keyword
       OR EXISTS(SELECT 1 FROM dbo.ReservationRooms rr2
                 JOIN dbo.Rooms rm2 ON rm2.RoomId=rr2.RoomId
                 WHERE rr2.ReservationId=r.ReservationId AND rm2.RoomNumber LIKE @pattern))
ORDER BY r.ReservationId DESC", connection))
            {
                string query = (keyword ?? "").Trim();
                cmd.Parameters.Add("@keyword", SqlDbType.NVarChar, 255).Value = query;
                cmd.Parameters.Add("@pattern", SqlDbType.NVarChar, 260).Value = "%" + query + "%";
                cmd.Parameters.Add("@checkin", SqlDbType.Bit).Value = checkInOnly;
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd)) adapter.Fill(result);
            }
            return result;
        }
    }
}
