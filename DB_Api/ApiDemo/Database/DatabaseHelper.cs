using Npgsql;
using System.Data;
using System.Data.SqlClient;

namespace ApiDemo.Database
{
    public class DatabaseHelper
    {
        // Database connection string
        private readonly string _connectionString;

        // SqlConnection object
        private NpgsqlConnection connection;

        // Constructor to initialize the parameters
        public DatabaseHelper()
        {
            _connectionString = File.ReadAllText("Database/db_config.cf");

            Connect();
        }

        // Method to establish a connection to the database
        public bool Connect()
        {
            if (connection == null)
            {
                connection = new NpgsqlConnection(this._connectionString);
            }

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
                Console.WriteLine("Connection opened successfully.");
                return true;
            }

            return false;
        }

        // Method to disconnect from the database
        public bool Disconnect()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
                Console.WriteLine("Connection closed successfully.");
                return true;
            }

            return false;
        }

        // Method to execute a generic query
        public DataTable? ExecuteQuery(string query)
        {
            DataTable dataTable = new DataTable();

            try
            {
                if (connection == null || connection.State != ConnectionState.Open)
                {
                    throw new InvalidOperationException("Connection must be open to execute queries.");
                }

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }

            return dataTable;
        }
    }
}
