using GestionTransport.FrontOffice.Models;
using GestionTransport.FrontOffice.Repositories.Interfaces;
using GestionTransport.FrontOffice.Services;
using Microsoft.Data.SqlClient;

namespace GestionTransport.FrontOffice.Repositories
{
    public class TypeTransportRepository : BaseRepository<TypeTransportModel>, ITypeTransportRepository
    {
        protected override string TableName => "TypeTransport";

        public TypeTransportRepository(IDatabaseService dbService) : base(dbService) { }

        protected override TypeTransportModel MapEntity(SqlDataReader reader)
        {
            return new TypeTransportModel
            {
                Id = (int)reader["Id"],
                Libelle = reader["Libelle"]?.ToString(),
                Actif = (bool)reader["Actif"]
            };
        }

        public List<TypeTransportModel> GetAll()
        {
            var types = new List<TypeTransportModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM TypeTransport ORDER BY Libelle", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    types.Add(MapEntity(reader));
                }
            }

            return types;
        }

        public TypeTransportModel GetById(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM TypeTransport WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return MapEntity(reader);
                }
            }

            return null;
        }

        public int Create(TypeTransportModel typeTransport)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"INSERT INTO TypeTransport (Libelle, Actif) 
                      OUTPUT INSERTED.Id
                      VALUES (@Libelle, @Actif)",
                    conn);

                AddParameter(cmd, "@Libelle", typeTransport.Libelle);
                AddParameter(cmd, "@Actif", typeTransport.Actif);

                return (int)cmd.ExecuteScalar();
            }
        }

        public void Update(TypeTransportModel typeTransport)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"UPDATE TypeTransport 
                      SET Libelle = @Libelle,
                          Actif = @Actif
                      WHERE Id = @Id",
                    conn);

                AddParameter(cmd, "@Id", typeTransport.Id);
                AddParameter(cmd, "@Libelle", typeTransport.Libelle);
                AddParameter(cmd, "@Actif", typeTransport.Actif);

                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM TypeTransport WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public List<TypeTransportModel> GetActifs()
        {
            var types = new List<TypeTransportModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM TypeTransport WHERE Actif = 1 ORDER BY Libelle", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    types.Add(MapEntity(reader));
                }
            }

            return types;
        }

        public TypeTransportModel GetByLibelle(string libelle)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM TypeTransport WHERE Libelle = @Libelle", conn);
                AddParameter(cmd, "@Libelle", libelle);
                
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return MapEntity(reader);
                }
            }

            return null;
        }

        // Override car cette table n'a pas DateDesactivation
        public override void Activate(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE TypeTransport SET Actif = 1 WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public override void Deactivate(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE TypeTransport SET Actif = 0 WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}