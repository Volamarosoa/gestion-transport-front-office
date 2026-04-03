using GestionTransport.FrontOffice.Models.Employe;
using GestionTransport.FrontOffice.Repositories.Interfaces;
using GestionTransport.FrontOffice.Services;
using Microsoft.Data.SqlClient;

namespace GestionTransport.FrontOffice.Repositories
{
    public class AdresseEmployeRepository : BaseRepository<AdresseEmployeModel>, IAdresseEmployeRepository
    {
        protected override string TableName => "AdresseEmploye";

        public AdresseEmployeRepository(IDatabaseService dbService) : base(dbService) { }

        protected override AdresseEmployeModel MapEntity(SqlDataReader reader)
        {
            return new AdresseEmployeModel
            {
                Id = (int)reader["Id"],
                IdEmploye = (int)reader["IdEmploye"],
                Adresse = reader["Adresse"]?.ToString(),
                Latitude = reader["Latitude"] == DBNull.Value 
                    ? null 
                    : (decimal?)reader["Latitude"],
                Longitude = reader["Longitude"] == DBNull.Value 
                    ? null 
                    : (decimal?)reader["Longitude"],
                EstPrincipale = (bool)reader["EstPrincipale"],
                Actif = (bool)reader["Actif"],
                DateInsertion = (DateTime)reader["DateInsertion"]
            };
        }

        public List<AdresseEmployeModel> GetAll()
        {
            var adresses = new List<AdresseEmployeModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM AdresseEmploye ORDER BY IdEmploye, EstPrincipale DESC", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    adresses.Add(MapEntity(reader));
                }
            }

            return adresses;
        }

        public AdresseEmployeModel GetById(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM AdresseEmploye WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return MapEntity(reader);
                }
            }

            return null;
        }

        public int Create(AdresseEmployeModel adresse)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                
                // Si cette adresse est définie comme principale, retirer le flag des autres
                if (adresse.EstPrincipale)
                {
                    var cmdUnset = new SqlCommand(
                        "UPDATE AdresseEmploye SET EstPrincipale = 0 WHERE IdEmploye = @IdEmploye", 
                        conn);
                    AddParameter(cmdUnset, "@IdEmploye", adresse.IdEmploye);
                    cmdUnset.ExecuteNonQuery();
                }

                var cmd = new SqlCommand(
                    @"INSERT INTO AdresseEmploye (IdEmploye, Adresse, Latitude, Longitude, EstPrincipale, Actif, DateInsertion) 
                      OUTPUT INSERTED.Id
                      VALUES (@IdEmploye, @Adresse, @Latitude, @Longitude, @EstPrincipale, @Actif, @DateInsertion)",
                    conn);

                AddParameter(cmd, "@IdEmploye", adresse.IdEmploye);
                AddParameter(cmd, "@Adresse", adresse.Adresse);
                AddParameter(cmd, "@Latitude", adresse.Latitude);
                AddParameter(cmd, "@Longitude", adresse.Longitude);
                AddParameter(cmd, "@EstPrincipale", adresse.EstPrincipale);
                AddParameter(cmd, "@Actif", adresse.Actif);
                AddParameter(cmd, "@DateInsertion", adresse.DateInsertion);

                return (int)cmd.ExecuteScalar();
            }
        }

        public void Update(AdresseEmployeModel adresse)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                // Si cette adresse est définie comme principale, retirer le flag des autres
                if (adresse.EstPrincipale)
                {
                    var cmdUnset = new SqlCommand(
                        "UPDATE AdresseEmploye SET EstPrincipale = 0 WHERE IdEmploye = @IdEmploye AND Id != @Id", 
                        conn);
                    AddParameter(cmdUnset, "@IdEmploye", adresse.IdEmploye);
                    AddParameter(cmdUnset, "@Id", adresse.Id);
                    cmdUnset.ExecuteNonQuery();
                }

                var cmd = new SqlCommand(
                    @"UPDATE AdresseEmploye 
                      SET Adresse = @Adresse,
                          Latitude = @Latitude,
                          Longitude = @Longitude,
                          EstPrincipale = @EstPrincipale
                      WHERE Id = @Id",
                    conn);

                AddParameter(cmd, "@Id", adresse.Id);
                AddParameter(cmd, "@Adresse", adresse.Adresse);
                AddParameter(cmd, "@Latitude", adresse.Latitude);
                AddParameter(cmd, "@Longitude", adresse.Longitude);
                AddParameter(cmd, "@EstPrincipale", adresse.EstPrincipale);

                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM AdresseEmploye WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public List<AdresseEmployeModel> GetByEmploye(int idEmploye)
        {
            var adresses = new List<AdresseEmployeModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT * FROM AdresseEmploye WHERE IdEmploye = @IdEmploye ORDER BY EstPrincipale DESC, DateInsertion DESC", 
                    conn);
                AddParameter(cmd, "@IdEmploye", idEmploye);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    adresses.Add(MapEntity(reader));
                }
            }

            return adresses;
        }

        public List<AdresseEmployeModel> GetActifsByEmploye(int idEmploye)
        {
            var adresses = new List<AdresseEmployeModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"SELECT * FROM AdresseEmploye 
                      WHERE IdEmploye = @IdEmploye AND Actif = 1 
                      ORDER BY EstPrincipale DESC, DateInsertion DESC", 
                    conn);
                AddParameter(cmd, "@IdEmploye", idEmploye);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    adresses.Add(MapEntity(reader));
                }
            }

            return adresses;
        }

        public AdresseEmployeModel GetAdressePrincipale(int idEmploye)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"SELECT TOP 1 * FROM AdresseEmploye 
                      WHERE IdEmploye = @IdEmploye 
                      AND EstPrincipale = 1 
                      AND Actif = 1", 
                    conn);
                AddParameter(cmd, "@IdEmploye", idEmploye);
                
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return MapEntity(reader);
                }
            }

            return null;
        }

        public void SetAsMain(int id, int idEmploye)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                // Retirer le flag principal de toutes les adresses de cet employé
                var cmdUnset = new SqlCommand(
                    "UPDATE AdresseEmploye SET EstPrincipale = 0 WHERE IdEmploye = @IdEmploye", 
                    conn);
                AddParameter(cmdUnset, "@IdEmploye", idEmploye);
                cmdUnset.ExecuteNonQuery();

                // Définir cette adresse comme principale
                var cmdSet = new SqlCommand(
                    "UPDATE AdresseEmploye SET EstPrincipale = 1 WHERE Id = @Id", 
                    conn);
                AddParameter(cmdSet, "@Id", id);
                cmdSet.ExecuteNonQuery();
            }
        }

        public List<AdresseEmployeModel> GetAvecCoordonnees()
        {
            var adresses = new List<AdresseEmployeModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"SELECT * FROM AdresseEmploye 
                      WHERE Latitude IS NOT NULL 
                      AND Longitude IS NOT NULL 
                      AND Actif = 1 
                      ORDER BY IdEmploye", 
                    conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    adresses.Add(MapEntity(reader));
                }
            }

            return adresses;
        }

        // AdresseEmploye table has no DateDesactivation column per base.sql
        public override void Activate(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE AdresseEmploye SET Actif = 1 WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public override void Deactivate(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE AdresseEmploye SET Actif = 0 WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
