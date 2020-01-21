using MySql.Data.MySqlClient;
using Plus.Database.Interfaces;

namespace Plus.Database
{
    public static class ConnectionProvider
    {
        public static string ConnectionString { get; set; }

        //TODO: Once all the queries are done within dao's, make this class none static
        //and inject it instead to the dao. Add some more async operations aswell.
        public static IAsyncDbClient GetConnection() =>
            new AsyncDatabaseClient(new MySqlConnection(ConnectionString));

        public static bool IsConnected()
        {
            try
            {
                MySqlConnection con = new MySqlConnection(ConnectionString);
                con.Open();
                MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT 1+1";
                cmd.ExecuteNonQuery();

                cmd.Dispose();
                con.Close();
            }
            catch (MySqlException)
            {
                return false;
            }

            return true;
        }
    }
}
