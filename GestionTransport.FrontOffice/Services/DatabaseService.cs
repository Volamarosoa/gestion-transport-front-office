using Microsoft.Data.SqlClient;

namespace GestionTransport.FrontOffice.Services
{
    public interface IDatabaseService
    {
        SqlConnection GetConnection();
        string GetConnectionString();
    }

    public class DatabaseService : IDatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }
    }
}