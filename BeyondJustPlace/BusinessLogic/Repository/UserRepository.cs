using System;
using System.Data;
using System.Data.SqlClient;

namespace BusinessLogic.Repository
{
    public class UserRepository
    {
        private readonly string connectionString=DatabaseConfig.ConnectionString;
        public string Login(string username,string password)
        {
            using(SqlConnection c=new SqlConnection(connectionString))
            {
                c.Open();
                using(SqlCommand cmd=new SqlCommand("SELECT UserId,Password,Role FROM dbo.Users WHERE Username=@name AND IsActive=1",c))
                {
                    cmd.Parameters.Add("@name",SqlDbType.NVarChar,50).Value=(username??"").Trim();
                    using(SqlDataReader reader=cmd.ExecuteReader())
                    {
                        if(!reader.Read())return "";
                        int id=reader.GetInt32(0);
                        string saved=reader.GetString(1),role=reader.GetString(2);
                        if(!PasswordSecurity.Verify(password,saved))return "";
                        reader.Close();
                        if(PasswordSecurity.IsLegacy(saved))
                        {
                            using(SqlCommand upgrade=new SqlCommand("UPDATE dbo.Users SET Password=@hash WHERE UserId=@id AND Password=@old",c))
                            {
                                upgrade.Parameters.Add("@hash",SqlDbType.NVarChar,300).Value=PasswordSecurity.Hash(password);
                                upgrade.Parameters.Add("@id",SqlDbType.Int).Value=id;
                                upgrade.Parameters.Add("@old",SqlDbType.NVarChar,300).Value=saved;
                                upgrade.ExecuteNonQuery();
                            }
                        }
                        return role;
                    }
                }
            }
        }
    }
}
