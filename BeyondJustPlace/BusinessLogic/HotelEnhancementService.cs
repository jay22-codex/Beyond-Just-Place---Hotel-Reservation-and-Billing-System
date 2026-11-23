using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BusinessLogic
{
    // All of the operations here run against each PC's local BeyondJustPlaceDB.
    // No form needs to open SQL connections just to render in Visual Studio Designer.
    public class HotelEnhancementService
    {
        private readonly string connectionString = DatabaseConfig.ConnectionString;

        public DataTable GetRoomTypes(bool includeInactive = false)
        {
            using (SqlConnection c = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(
                "SELECT RoomTypeId, TypeName, Rate, Capacity, BedCount, SizeSqm, ReservationFee, Description, IsActive " +
                "FROM dbo.RoomTypes WHERE @include=1 OR IsActive=1 ORDER BY TypeName", c))
            {
                cmd.Parameters.Add("@include", SqlDbType.Bit).Value = includeInactive;
                return Fill(c, cmd);
            }
        }

        public void SaveRoomType(int id, string name, decimal rate, int capacity,
            int beds, int sizeSqm, decimal fee, string description, bool active)
        {
            name = (name ?? "").Trim();
            description = (description ?? "").Trim();
            if (name.Length == 0 || name.Length > 100 || rate < 0 || capacity < 1 ||
                beds < 1 || beds > capacity || sizeSqm < 1 || fee < 0 || description.Length > 500)
                throw new InvalidOperationException("Room type needs a name, valid rate, beds, capacity, size and description.");
            if (description.Length == 0)
                throw new InvalidOperationException("Provide a short room description.");
            using (SqlConnection c = new SqlConnection(connectionString))
            {
                c.Open();
                using (SqlTransaction tx = c.BeginTransaction())
                {
                    try
                    {
                        string oldName = null;
                        if (id > 0)
                        {
                            using (SqlCommand old = new SqlCommand("SELECT TypeName FROM dbo.RoomTypes WHERE RoomTypeId=@id", c, tx))
                            {
                                old.Parameters.Add("@id", SqlDbType.Int).Value = id;
                                oldName = Convert.ToString(old.ExecuteScalar());
                                if (string.IsNullOrWhiteSpace(oldName)) throw new InvalidOperationException("Room type not found.");
                            }
                        }
                        using (SqlCommand duplicate = new SqlCommand(
                            "SELECT COUNT(*) FROM dbo.RoomTypes WHERE TypeName=@name AND RoomTypeId<>@id", c, tx))
                        {
                            duplicate.Parameters.Add("@name", SqlDbType.NVarChar, 100).Value = name;
                            duplicate.Parameters.Add("@id", SqlDbType.Int).Value = id;
                            if (Convert.ToInt32(duplicate.ExecuteScalar()) > 0)
                                throw new InvalidOperationException("This room type already exists.");
                        }
                        string sql = id > 0
                            ? "UPDATE dbo.RoomTypes SET TypeName=@name, Rate=@rate,Capacity=@capacity,BedCount=@beds,SizeSqm=@size,ReservationFee=@fee,Description=@description,IsActive=@active WHERE RoomTypeId=@id"
                            : "INSERT INTO dbo.RoomTypes(TypeName,Rate,Capacity,BedCount,SizeSqm,ReservationFee,Description,IsActive) VALUES(@name,@rate,@capacity,@beds,@size,@fee,@description,@active)";
                        using (SqlCommand cmd = new SqlCommand(sql, c, tx))
                        {
                            cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                            cmd.Parameters.Add("@name", SqlDbType.NVarChar, 100).Value = name;
                            AddDecimal(cmd,"@rate",rate);
                            AddDecimal(cmd,"@fee",fee);
                            cmd.Parameters.Add("@capacity", SqlDbType.Int).Value = capacity;
                            cmd.Parameters.Add("@beds", SqlDbType.Int).Value = beds;
                            cmd.Parameters.Add("@size", SqlDbType.Int).Value = sizeSqm;
                            cmd.Parameters.Add("@description", SqlDbType.NVarChar, 500).Value = description;
                            cmd.Parameters.Add("@active", SqlDbType.Bit).Value = active;
                            cmd.ExecuteNonQuery();
                        }
                        // Keep existing room records/history, synchronize current room setup.
                        if (id > 0)
                        {
                            using (SqlCommand cmd = new SqlCommand(
                                "UPDATE dbo.Rooms SET RoomType=@name,RoomRate=@rate,RoomCapacity=@capacity,BedCount=@beds,SizeSqm=@size,ReservationFee=@fee,Description=@description WHERE RoomType=@oldName", c, tx))
                            {
                                cmd.Parameters.Add("@name", SqlDbType.NVarChar, 100).Value = name;
                                cmd.Parameters.Add("@oldName", SqlDbType.NVarChar, 100).Value = oldName;
                                AddDecimal(cmd,"@rate",rate);
                                AddDecimal(cmd,"@fee",fee);
                                cmd.Parameters.Add("@capacity", SqlDbType.Int).Value = capacity;
                                cmd.Parameters.Add("@beds", SqlDbType.Int).Value = beds;
                                cmd.Parameters.Add("@size", SqlDbType.Int).Value = sizeSqm;
                                cmd.Parameters.Add("@description", SqlDbType.NVarChar, 500).Value = description;
                                cmd.ExecuteNonQuery();
                            }
                        }
                        tx.Commit();
                    }
                    catch { try { tx.Rollback(); } catch { } throw; }
                }
            }
        }

        public void SetRoomTypeActive(int id, bool active)
        {
            using (SqlConnection c = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("UPDATE dbo.RoomTypes SET IsActive=@active WHERE RoomTypeId=@id", c))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@active", SqlDbType.Bit).Value = active;
                c.Open();
                if (cmd.ExecuteNonQuery() != 1) throw new InvalidOperationException("Room type not found.");
            }
        }

        public DataTable GetAvailabilityForDate(DateTime date)
        {
            using(SqlConnection c=new SqlConnection(connectionString))
            using(SqlCommand cmd=new SqlCommand(@"
SELECT rm.RoomNumber AS [Room Number],rm.RoomType AS [Room Type],
       rm.RoomCapacity AS [Capacity],rm.BedCount AS [Beds],rm.SizeSqm AS [Size (sqm)],
       CASE WHEN rm.IsActive=0 OR rt.IsActive=0 THEN N'Inactive'
            WHEN rm.RoomStatus<>N'Available' THEN rm.RoomStatus
            WHEN EXISTS(SELECT 1 FROM dbo.Reservations r WHERE r.ReservationStatus IN(N'Pending',N'Confirmed',N'Checked In')
                     AND r.CheckInDate<=@date AND r.CheckOutDate>@date
                     AND (EXISTS(SELECT 1 FROM dbo.ReservationRooms rr WHERE rr.ReservationId=r.ReservationId AND rr.RoomId=rm.RoomId)
                       OR (NOT EXISTS(SELECT 1 FROM dbo.ReservationRooms rr WHERE rr.ReservationId=r.ReservationId) AND r.RoomId=rm.RoomId)))
            THEN N'Reserved' ELSE N'Available' END AS [Availability],
       rm.RoomRate AS [Rate / Night]
FROM dbo.Rooms rm LEFT JOIN dbo.RoomTypes rt ON rt.TypeName=rm.RoomType
ORDER BY rm.RoomNumber",c))
            {
                cmd.Parameters.Add("@date",SqlDbType.Date).Value=date.Date;
                return Fill(c,cmd);
            }
        }

        public DataTable GetGuests(string keyword, bool includeInactive = false)
        {
            using (SqlConnection c = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 500 GuestId,GuestName,ContactNumber,Email,HomeAddress,IdType,
       IdNumber,IdVerified,IsActive,CreatedAt
FROM dbo.Guests
WHERE (@all=1 OR IsActive=1)
  AND (GuestName LIKE @kw OR ContactNumber LIKE @kw OR ISNULL(Email,N'') LIKE @kw OR ISNULL(IdNumber,N'') LIKE @kw)
ORDER BY GuestId DESC", c))
            {
                cmd.Parameters.Add("@all", SqlDbType.Bit).Value = includeInactive;
                cmd.Parameters.Add("@kw", SqlDbType.NVarChar, 255).Value = "%"+(keyword??"").Trim()+"%";
                return Fill(c,cmd);
            }
        }

        public int CreateDemoGuestsForSchool()
        {
            int added=0;
            using(SqlConnection c=new SqlConnection(connectionString))
            {
                c.Open();
                for(int i=1;i<=50;i++)
                {
                    string name="DEMO Guest " + i.ToString("D2");
                    using(SqlCommand cmd=new SqlCommand(@"
IF NOT EXISTS(SELECT 1 FROM dbo.Guests WHERE GuestName=@name)
BEGIN
 INSERT INTO dbo.Guests(GuestName,ContactNumber,Email,HomeAddress,IdType,IdNumber)
 VALUES(@name,@contact,@email,N'Demonstration only',N'Sample ID',@number);
 SELECT 1;
END ELSE SELECT 0;",c))
                    {
                        cmd.Parameters.Add("@name",SqlDbType.NVarChar,255).Value=name;
                        cmd.Parameters.Add("@contact",SqlDbType.NVarChar,50).Value="DEMO-"+i.ToString("D4");
                        cmd.Parameters.Add("@email",SqlDbType.NVarChar,254).Value="demo"+i.ToString("D2")+"@example.invalid";
                        cmd.Parameters.Add("@number",SqlDbType.NVarChar,100).Value="DEMO-ID-"+i.ToString("D4");
                        added+=Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            return added;
        }

        public void SetGuestActive(int guestId, bool active)
        {
            using (SqlConnection c = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("UPDATE dbo.Guests SET IsActive=@active WHERE GuestId=@id",c))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value=guestId;
                cmd.Parameters.Add("@active", SqlDbType.Bit).Value=active;
                c.Open();
                if (cmd.ExecuteNonQuery()!=1) throw new InvalidOperationException("Guest not found.");
            }
        }

        public void SaveGuestIdentity(int guestId, string idNumber, bool idVerified)
        {
            idNumber = (idNumber ?? "").Trim();
            if (idNumber.Length > 100 || (idVerified && idNumber.Length == 0))
                throw new InvalidOperationException("Enter the guest's valid ID number before marking ID checked.");
            using (SqlConnection c = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("UPDATE dbo.Guests SET IdNumber=@number,IdVerified=@checked WHERE GuestId=@id AND IsActive=1",c))
            {
                cmd.Parameters.Add("@id",SqlDbType.Int).Value=guestId;
                cmd.Parameters.Add("@number",SqlDbType.NVarChar,100).Value = idNumber.Length==0?(object)DBNull.Value:idNumber;
                cmd.Parameters.Add("@checked",SqlDbType.Bit).Value = idVerified;
                c.Open();
                if (cmd.ExecuteNonQuery()!=1) throw new InvalidOperationException("Active guest record not found.");
            }
        }

        public DataTable GetReservationDetails(int reservationId)
        {
            using (SqlConnection c = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT r.ReservationId, r.GuestId, g.GuestName,g.ContactNumber,g.Email,g.HomeAddress,g.IdType,g.IdNumber,g.IdVerified,
 r.CheckInDate,r.CheckOutDate,r.NumberOfGuests,r.Adults,r.Children,r.RoomCount,r.SpecialRequest,r.ReservationStatus,
 r.IdentityChecked,r.KeysIssued,r.ReservationFeeAmount,r.ReservationFeeStatus,r.ReservationFeeReference,
 COALESCE(STUFF((SELECT N', '+rrm.RoomNumber+N' ('+rrm.RoomType+N')'
  FROM dbo.ReservationRooms rrr INNER JOIN dbo.Rooms rrm ON rrm.RoomId=rrr.RoomId
  WHERE rrr.ReservationId=r.ReservationId FOR XML PATH(''),TYPE).value('.','nvarchar(max)'),1,2,N''),rm.RoomNumber) AS Rooms
FROM dbo.Reservations r
JOIN dbo.Guests g ON g.GuestId=r.GuestId
JOIN dbo.Rooms rm ON rm.RoomId=r.RoomId
WHERE r.ReservationId=@id",c))
            {
                cmd.Parameters.Add("@id",SqlDbType.Int).Value=reservationId;
                return Fill(c,cmd);
            }
        }

        public void ConfirmKeyIssuance(int reservationId, bool idChecked, bool keysIssued)
        {
            if (!idChecked || !keysIssued)
                throw new InvalidOperationException("Check the guest's physical ID and confirm room key handover.");
            using (SqlConnection c = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
UPDATE r SET IdentityChecked=1,KeysIssued=1
FROM dbo.Reservations r JOIN dbo.Guests g ON g.GuestId=r.GuestId
WHERE r.ReservationId=@id AND r.ReservationStatus='Checked In' AND g.IdVerified=1",c))
            {
                cmd.Parameters.Add("@id",SqlDbType.Int).Value=reservationId;
                c.Open();
                if(cmd.ExecuteNonQuery()!=1) throw new InvalidOperationException("Verify the guest's ID record before issuing keys.");
            }
        }

        public void UpdateReservationStatus(int id,string status)
        {
            if(status!="Confirmed" && status!="Cancelled" && status!="Pending")
                throw new InvalidOperationException("Choose Pending, Confirmed, or Cancelled.");
            using(SqlConnection c=new SqlConnection(connectionString))
            using(SqlCommand cmd=new SqlCommand(@"
UPDATE dbo.Reservations SET ReservationStatus=@status
WHERE ReservationId=@id AND ReservationStatus IN(N'Pending',N'Confirmed')
AND (@status<>N'Confirmed' OR ISNULL(ReservationFeeAmount,0)=0 OR ReservationFeeStatus=N'Paid')
AND (@status<>N'Cancelled' OR ReservationFeeStatus<>N'Paid')",c))
            {
                cmd.Parameters.Add("@id",SqlDbType.Int).Value=id;
                cmd.Parameters.Add("@status",SqlDbType.NVarChar,30).Value=status;
                c.Open();
                if(cmd.ExecuteNonQuery()!=1)throw new InvalidOperationException("Cannot change reservation status. For Confirmed, paid deposits must be verified first.");
            }
        }

        public void ConfirmReservationFee(int reservationId, string method, string reference, bool verified)
        {
            if (!verified) throw new InvalidOperationException("Payment must be verified by the staff before confirming.");
            if (method!="GCash" && method!="Cash" && method!="Card") throw new InvalidOperationException("Select a valid payment method.");
            reference = (reference??"").Trim();
            if (method=="GCash" && (reference.Length<6 || reference.Length>100))
                throw new InvalidOperationException("Enter the GCash transaction reference (at least 6 characters).");
            using (SqlConnection c=new SqlConnection(connectionString))
            using(SqlCommand cmd=new SqlCommand(@"
UPDATE dbo.Reservations
SET ReservationFeeStatus=N'Paid',ReservationFeeReference=@reference,
    ReservationFeePaymentMethod=@method,
    ReservationStatus=N'Confirmed'
WHERE ReservationId=@id AND ReservationStatus=N'Pending' AND ISNULL(ReservationFeeAmount,0)>0
 AND ISNULL(ReservationFeeStatus,N'Unpaid')<>N'Paid'",c))
            {
                cmd.Parameters.Add("@id",SqlDbType.Int).Value=reservationId;
                cmd.Parameters.Add("@method",SqlDbType.NVarChar,30).Value=method;
                cmd.Parameters.Add("@reference",SqlDbType.NVarChar,100).Value=reference.Length==0?(object)DBNull.Value:reference;
                c.Open();
                if (method=="GCash")
                {
                    using(SqlCommand used = new SqlCommand(
                        "SELECT COUNT(*) FROM dbo.Payments WHERE PaymentReference=@ref",c))
                    {
                        used.Parameters.Add("@ref",SqlDbType.NVarChar,100).Value=reference;
                        if(Convert.ToInt32(used.ExecuteScalar())>0)
                            throw new InvalidOperationException(
                                "This GCash reference was already used for a checkout payment.");
                    }
                }
                try{if(cmd.ExecuteNonQuery()!=1)throw new InvalidOperationException("Reservation is not pending or fee was already paid.");}
                catch(SqlException ex) when(ex.Number==2601 || ex.Number==2627)
                {throw new InvalidOperationException("This GCash reference number was already used. Verify the transaction.");}
            }
        }

        public void AddServiceCharge(int reservationId, string service, int quantity, decimal unitPrice)
        {
            if (service!="Food" && service!="Laundry")
                throw new InvalidOperationException("Only Food or Laundry add-ons are supported.");
            if (quantity<1 || quantity>100 || unitPrice<0 || unitPrice>100000)
                throw new InvalidOperationException("Enter a valid quantity and unit price.");
            using(SqlConnection c=new SqlConnection(connectionString))
            using(SqlCommand cmd=new SqlCommand(@"
INSERT INTO dbo.ReservationServices(ReservationId,ServiceName,Quantity,UnitPrice)
SELECT @id,@service,@qty,@price FROM dbo.Reservations r
WHERE r.ReservationId=@id AND r.ReservationStatus=N'Checked In'",c))
            {
                cmd.Parameters.Add("@id",SqlDbType.Int).Value=reservationId;
                cmd.Parameters.Add("@service",SqlDbType.NVarChar,30).Value=service;
                cmd.Parameters.Add("@qty",SqlDbType.Int).Value=quantity;
                AddDecimal(cmd,"@price",unitPrice);
                c.Open();
                if(cmd.ExecuteNonQuery()!=1)throw new InvalidOperationException("You can add charges only to a checked-in reservation.");
            }
        }

        public DataTable GetServiceCharges(int reservationId)
        {
            using(SqlConnection c=new SqlConnection(connectionString))
            using(SqlCommand cmd=new SqlCommand(@"
SELECT ServiceName AS [Service],Quantity AS [Qty],UnitPrice AS [Unit Price],
       Quantity*UnitPrice AS [Amount],CreatedAt AS [Date]
FROM dbo.ReservationServices WHERE ReservationId=@id ORDER BY ServiceId",c))
            {
                cmd.Parameters.Add("@id",SqlDbType.Int).Value=reservationId;
                return Fill(c,cmd);
            }
        }

        public void SetUserActive(int userId, bool active, string actingUsername)
        {
            using(SqlConnection c=new SqlConnection(connectionString))
            {
                c.Open();
                using(SqlTransaction tx=c.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        string username=null,role=null;
                        using(SqlCommand get=new SqlCommand("SELECT Username,Role FROM dbo.Users WITH(UPDLOCK,HOLDLOCK) WHERE UserId=@id",c,tx))
                        {
                            get.Parameters.Add("@id",SqlDbType.Int).Value=userId;
                            using(SqlDataReader r=get.ExecuteReader())
                            {if(r.Read()){username=r.GetString(0);role=r.GetString(1);}}
                        }
                        if(username==null)throw new InvalidOperationException("User not found.");
                        if(!active && username.Equals(actingUsername,StringComparison.OrdinalIgnoreCase))
                            throw new InvalidOperationException("You cannot deactivate your own signed-in account.");
                        if(!active && role=="Admin")
                        {
                            using(SqlCommand chk=new SqlCommand("SELECT COUNT(*) FROM dbo.Users WHERE Role=N'Admin' AND IsActive=1",c,tx))
                                if(Convert.ToInt32(chk.ExecuteScalar())<2)throw new InvalidOperationException("At least one active Admin is required.");
                        }
                        using(SqlCommand upd=new SqlCommand("UPDATE dbo.Users SET IsActive=@active WHERE UserId=@id",c,tx))
                        {
                            upd.Parameters.Add("@active",SqlDbType.Bit).Value=active;
                            upd.Parameters.Add("@id",SqlDbType.Int).Value=userId;
                            upd.ExecuteNonQuery();
                        }
                        tx.Commit();
                    }
                    catch{try{tx.Rollback();}catch{}throw;}
                }
            }
        }

        public void SetRoomActive(int roomId,bool active)
        {
            using(SqlConnection c=new SqlConnection(connectionString))
            using(SqlCommand cmd=new SqlCommand("UPDATE dbo.Rooms SET IsActive=@active WHERE RoomId=@id",c))
            {
                cmd.Parameters.Add("@active",SqlDbType.Bit).Value=active;
                cmd.Parameters.Add("@id",SqlDbType.Int).Value=roomId;
                c.Open();
                if(cmd.ExecuteNonQuery()!=1)throw new InvalidOperationException("Room not found.");
            }
        }

        public void SetRoomDetails(int roomId,int beds,int sizeSqm,string description)
        {
            if(beds<1 || sizeSqm<1 || string.IsNullOrWhiteSpace(description) || description.Trim().Length>500)
                throw new InvalidOperationException("Beds and size must be positive; description must be under 500 characters.");
            using(SqlConnection c=new SqlConnection(connectionString))
            using(SqlCommand cmd=new SqlCommand("UPDATE dbo.Rooms SET BedCount=@beds,SizeSqm=@size,Description=@description WHERE RoomId=@id",c))
            {
                cmd.Parameters.Add("@id",SqlDbType.Int).Value=roomId;
                cmd.Parameters.Add("@beds",SqlDbType.Int).Value=beds;
                cmd.Parameters.Add("@size",SqlDbType.Int).Value=sizeSqm;
                cmd.Parameters.Add("@description",SqlDbType.NVarChar,500).Value=(description??"").Trim();
                c.Open();
                if(cmd.ExecuteNonQuery()!=1)throw new InvalidOperationException("Room not found.");
            }
        }

        private static void AddDecimal(SqlCommand cmd,string name,decimal value)
        {
            SqlParameter p=cmd.Parameters.Add(name,SqlDbType.Decimal);
            p.Precision=18;p.Scale=2;p.Value=value;
        }
        private static DataTable Fill(SqlConnection c,SqlCommand cmd)
        {
            DataTable data=new DataTable();
            using(SqlDataAdapter da=new SqlDataAdapter(cmd))da.Fill(data);
            return data;
        }
    }
}
