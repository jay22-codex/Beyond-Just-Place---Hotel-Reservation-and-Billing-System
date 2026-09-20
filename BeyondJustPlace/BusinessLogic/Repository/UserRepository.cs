using System.Configuration;
using System.Data.SqlClient;

namespace BusinessLogic.Repository
{
    public class UserRepository
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["HotelDB"].ConnectionString;

        public string Login(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query =
                    "SELECT Role FROM dbo.Users " +
                    "WHERE Username = @username AND Password = @password";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                object result = command.ExecuteScalar();

                if (result != null)
                {
                    return result.ToString();
                }

                return "";
            }
        }
    }
}