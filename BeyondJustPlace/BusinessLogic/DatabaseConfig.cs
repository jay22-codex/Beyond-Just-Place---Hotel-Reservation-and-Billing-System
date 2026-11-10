using System.Data.SqlClient;

namespace BusinessLogic
{
    /// <summary>
    /// One local SQL Server LocalDB configuration for the whole application.
    /// Every developer gets an independent database on his/her own Windows PC.
    /// </summary>
    public static class DatabaseConfig
    {
        public const string DatabaseName = "BeyondJustPlaceDB";
        public const string LocalDbInstance = @"(localdb)\MSSQLLocalDB";

        public static string MasterConnectionString
        {
            get
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = LocalDbInstance;
                builder.InitialCatalog = "master";
                builder.IntegratedSecurity = true;
                builder.ConnectTimeout = 30;    
                return builder.ConnectionString;
            }
        }

        public static string ConnectionString
        {
            get
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = LocalDbInstance;
                builder.InitialCatalog = DatabaseName;
                builder.IntegratedSecurity = true;
                builder.ConnectTimeout = 30;
                builder.MultipleActiveResultSets = true;
                return builder.ConnectionString;
            }
        }
    }
}
