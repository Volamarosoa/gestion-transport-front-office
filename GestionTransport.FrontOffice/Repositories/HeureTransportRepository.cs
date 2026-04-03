using GestionTransport.FrontOffice.Models.Transport;
using GestionTransport.FrontOffice.Repositories.Interfaces;
using GestionTransport.FrontOffice.Services;
using Microsoft.Data.SqlClient;

namespace GestionTransport.FrontOffice.Repositories
{
    public class HeureTransportRepository : BaseRepository<HeureTransportModel>, IHeureTransportRepository
    {
        protected override string TableName => "HeureTransport";

        public HeureTransportRepository(IDatabaseService dbService) : base(dbService) { }

        protected override HeureTransportModel MapEntity(SqlDataReader reader)
        {
            return new HeureTransportModel
            {
                Id = (int)reader["Id"],
                Heure = reader["Heure"] == DBNull.Value 
                    ? null 
                    : (TimeSpan?)reader["Heure"],
                Libelle = reader["Libelle"]?.ToString(),
                Actif = (bool)reader["Actif"],
                DateInsertion = (DateTime)reader["DateInsertion"]
            };
        }

        public List<HeureTransportModel> GetAll()
        {
            var heures = new List<HeureTransportModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM HeureTransport ORDER BY Heure", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    heures.Add(MapEntity(reader));
                }
            }

            return heures;
        }

        public HeureTransportModel GetById(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM HeureTransport WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return MapEntity(reader);
                }
            }

            return null;
        }

        public int Create(HeureTransportModel heureTransport)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"INSERT INTO HeureTransport (Heure, Libelle, Actif, DateInsertion) 
                      OUTPUT INSERTED.Id
                      VALUES (@Heure, @Libelle, @Actif, @DateInsertion)",
                    conn);

                AddParameter(cmd, "@Heure", heureTransport.Heure);
                AddParameter(cmd, "@Libelle", heureTransport.Libelle);
                AddParameter(cmd, "@Actif", heureTransport.Actif);
                AddParameter(cmd, "@DateInsertion", heureTransport.DateInsertion);

                return (int)cmd.ExecuteScalar();
            }
        }

        public void Update(HeureTransportModel heureTransport)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"UPDATE HeureTransport 
                      SET Heure = @Heure,
                          Libelle = @Libelle
                      WHERE Id = @Id",
                    conn);

                AddParameter(cmd, "@Id", heureTransport.Id);
                AddParameter(cmd, "@Heure", heureTransport.Heure);
                AddParameter(cmd, "@Libelle", heureTransport.Libelle);

                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM HeureTransport WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public List<HeureTransportModel> GetActifs()
        {
            var heures = new List<HeureTransportModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM HeureTransport WHERE Actif = 1 ORDER BY Heure", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    heures.Add(MapEntity(reader));
                }
            }

            return heures;
        }

        public List<HeureTransportModel> GetByPeriode(string libelle)
        {
            var heures = new List<HeureTransportModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT * FROM HeureTransport WHERE Libelle = @Libelle AND Actif = 1 ORDER BY Heure", 
                    conn);
                AddParameter(cmd, "@Libelle", libelle);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    heures.Add(MapEntity(reader));
                }
            }

            return heures;
        }

        public HeureTransportModel GetByHeure(TimeSpan heure)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM HeureTransport WHERE Heure = @Heure", conn);
                AddParameter(cmd, "@Heure", heure);
                
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return MapEntity(reader);
                }
            }

            return null;
        }

        // HeureTransport table has no DateDesactivation column per base.sql
        public override void Activate(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE HeureTransport SET Actif = 1 WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public override void Deactivate(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE HeureTransport SET Actif = 0 WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
