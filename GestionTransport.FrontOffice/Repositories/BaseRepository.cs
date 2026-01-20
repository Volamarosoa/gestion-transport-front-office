using GestionTransport.FrontOffice.Services;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GestionTransport.FrontOffice.Repositories
{
    public abstract class BaseRepository<T> where T : class
    {
        protected readonly IDatabaseService _dbService;
        protected abstract string TableName { get; }

        protected BaseRepository(IDatabaseService dbService)
        {
            _dbService = dbService;
        }

        protected SqlConnection GetConnection()
        {
            return _dbService.GetConnection();
        }

        protected T MapFromReader(SqlDataReader reader)
        {
            return MapEntity(reader);
        }

        protected abstract T MapEntity(SqlDataReader reader);

        protected void AddParameter(SqlCommand cmd, string name, object value)
        {
            cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
        }

        public virtual void Activate(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    $@"UPDATE {TableName} 
                       SET Actif = 1, DateDesactivation = NULL 
                       WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public virtual void Deactivate(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    $@"UPDATE {TableName} 
                       SET Actif = 0, DateDesactivation = GETDATE() 
                       WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}