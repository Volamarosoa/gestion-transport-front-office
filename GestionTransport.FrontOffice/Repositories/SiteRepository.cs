using GestionTransport.FrontOffice.Models;
using GestionTransport.FrontOffice.Repositories.Interfaces;
using GestionTransport.FrontOffice.Services;
using Microsoft.Data.SqlClient;

namespace GestionTransport.FrontOffice.Repositories
{
    public class SiteRepository : BaseRepository<SiteModel>, ISiteRepository
    {
        protected override string TableName => "Site";

        public SiteRepository(IDatabaseService dbService) : base(dbService) { }

        protected override SiteModel MapEntity(SqlDataReader reader)
        {
            return new SiteModel
            {
                Id = (int)reader["Id"],
                Nom = reader["Nom"]?.ToString(),
                Adresse = reader["Adresse"]?.ToString(),
                Latitude = reader["Latitude"] == DBNull.Value 
                    ? null 
                    : (decimal?)reader["Latitude"],
                Longitude = reader["Longitude"] == DBNull.Value 
                    ? null 
                    : (decimal?)reader["Longitude"],
                Actif = (bool)reader["Actif"],
                DateInsertion = (DateTime)reader["DateInsertion"]
            };
        }

        public List<SiteModel> GetAll()
        {
            var sites = new List<SiteModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Site ORDER BY Nom", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    sites.Add(MapEntity(reader));
                }
            }

            return sites;
        }

        public SiteModel GetById(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Site WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return MapEntity(reader);
                }
            }

            return null;
        }

        public int Create(SiteModel site)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"INSERT INTO Site (Nom, Adresse, Latitude, Longitude, Actif, DateInsertion) 
                      OUTPUT INSERTED.Id
                      VALUES (@Nom, @Adresse, @Latitude, @Longitude, @Actif, @DateInsertion)",
                    conn);

                AddParameter(cmd, "@Nom", site.Nom);
                AddParameter(cmd, "@Adresse", site.Adresse);
                AddParameter(cmd, "@Latitude", site.Latitude);
                AddParameter(cmd, "@Longitude", site.Longitude);
                AddParameter(cmd, "@Actif", site.Actif);
                AddParameter(cmd, "@DateInsertion", site.DateInsertion);

                return (int)cmd.ExecuteScalar();
            }
        }

        public void Update(SiteModel site)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"UPDATE Site 
                      SET Nom = @Nom, 
                          Adresse = @Adresse,
                          Latitude = @Latitude,
                          Longitude = @Longitude
                      WHERE Id = @Id",
                    conn);

                AddParameter(cmd, "@Id", site.Id);
                AddParameter(cmd, "@Nom", site.Nom);
                AddParameter(cmd, "@Adresse", site.Adresse);
                AddParameter(cmd, "@Latitude", site.Latitude);
                AddParameter(cmd, "@Longitude", site.Longitude);

                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM Site WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public List<SiteModel> GetActifs()
        {
            var sites = new List<SiteModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Site WHERE Actif = 1 ORDER BY Nom", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    sites.Add(MapEntity(reader));
                }
            }

            return sites;
        }

        public List<SiteModel> GetAvecCoordonnees()
        {
            var sites = new List<SiteModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT * FROM Site WHERE Latitude IS NOT NULL AND Longitude IS NOT NULL ORDER BY Nom", 
                    conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    sites.Add(MapEntity(reader));
                }
            }

            return sites;
        }

        public SiteModel GetByNom(string nom)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Site WHERE Nom = @Nom", conn);
                AddParameter(cmd, "@Nom", nom);
                
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return MapEntity(reader);
                }
            }

            return null;
        }

        // Site table has no DateDesactivation column per base.sql
        public override void Activate(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE Site SET Actif = 1 WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public override void Deactivate(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE Site SET Actif = 0 WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
