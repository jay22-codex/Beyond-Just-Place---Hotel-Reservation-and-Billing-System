using System.Data.SqlClient;

namespace BusinessLogic.Repository
{
    public class UserRepository
    {
        string connectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=BeyondJustPlaceDB;Integrated Security=True";

        public string Login(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query =
                    "SELECT Role FROM Users WHERE Username=@username AND Password=@password";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                object result = command.ExecuteScalar();

                if (result != null)
                    return result.ToString();

                return "";
            }
        }
    }
}