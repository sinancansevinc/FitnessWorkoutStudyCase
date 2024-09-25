using System.Data;

namespace FitnessWorkout.Data
{
    public class DatabaseConnection : IDisposable
    {
        private readonly string _connectionString;
        private MySql.Data.MySqlClient.MySqlConnection _connection;

        public DatabaseConnection(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection GetConnection()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                _connection = new MySql.Data.MySqlClient.MySqlConnection(_connectionString);
                _connection.Open();
            }
            return _connection;
        }

        public void Dispose()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }
    }
}
