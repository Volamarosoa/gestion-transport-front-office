using GestionTransport.FrontOffice.Models;
using GestionTransport.FrontOffice.Repositories.Interfaces;
using GestionTransport.FrontOffice.Services;
using Microsoft.Data.SqlClient;

namespace GestionTransport.FrontOffice.Repositories
{
    public class SiteRepository : BaseRepository<SiteModel>, ISiteRepository
    {
        protected override string TableName => "SITE";

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
                DateInsertion = (DateTime)reader["DateInsertion"],
                DateDesactivation = reader["DateDesactivation"] == DBNull.Value 
                    ? null 
                    : (DateTime?)reader["DateDesactivation"]
            };
        }

        public List<SiteModel> GetAll()
        {
            var sites = new List<SiteModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM SITE ORDER BY Nom", conn);
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
                var cmd = new SqlCommand("SELECT * FROM SITE WHERE Id = @Id", conn);
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
                    @"INSERT INTO SITE (Nom, Adresse, Latitude, Longitude, Actif, DateInsertion) 
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
                    @"UPDATE SITE 
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
                var cmd = new SqlCommand("DELETE FROM SITE WHERE Id = @Id", conn);
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
                var cmd = new SqlCommand("SELECT * FROM SITE WHERE Actif = 1 ORDER BY Nom", conn);
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
                    "SELECT * FROM SITE WHERE Latitude IS NOT NULL AND Longitude IS NOT NULL ORDER BY Nom", 
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
                var cmd = new SqlCommand("SELECT * FROM SITE WHERE Nom = @Nom", conn);
                AddParameter(cmd, "@Nom", nom);
                
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return MapEntity(reader);
                }
            }

            return null;
        }
    }
}