using GestionTransport.FrontOffice.Models.Affectation;
using GestionTransport.FrontOffice.Models;
using GestionTransport.FrontOffice.Models.Employe;
using GestionTransport.FrontOffice.Models.Transport;
using GestionTransport.FrontOffice.Repositories.Interfaces;
using GestionTransport.FrontOffice.Services;
using Microsoft.Data.SqlClient;

namespace GestionTransport.FrontOffice.Repositories
{
    public class AffectationRepository : BaseRepository<AffectationModel>, IAffectationRepository
    {
        protected override string TableName => "Affectation";

        public AffectationRepository(IDatabaseService dbService) : base(dbService) { }

        protected override AffectationModel MapEntity(SqlDataReader reader)
        {
            return new AffectationModel
            {
                Id = (int)reader["Id"],
                DateTransport = reader["DateTransport"] == DBNull.Value
                    ? null
                    : (DateTime?)reader["DateTransport"],
                IdEmploye = (int)reader["IdEmploye"],
                IdAdresse = (int)reader["IdAdresse"],
                IdTypeTransport = (int)reader["IdTypeTransport"],
                IdSite = (int)reader["IdSite"],
                IdVehicule = reader["IdVehicule"] == DBNull.Value 
                    ? null 
                    : (int?)reader["IdVehicule"],
                IdHeureTransport = (int)reader["IdHeureTransport"],
                EstValidee = reader["EstValidee"] == DBNull.Value 
                    ? null 
                    : (bool?)reader["EstValidee"],
                Commentaire = reader["Commentaire"]?.ToString(),
                DateCreation = (DateTime)reader["DateCreation"],
                DateValidation = reader["DateValidation"] == DBNull.Value 
                    ? null 
                    : (DateTime?)reader["DateValidation"],
                IdType = (int)reader["IdType"],
                EstArchive = (bool)reader["EstArchive"]
            };
        }

        public List<AffectationModel> GetAll()
        {
            var affectations = new List<AffectationModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Affectation ORDER BY DateCreation DESC", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    affectations.Add(MapEntity(reader));
                }
            }

            return affectations;
        }

        public AffectationModel GetById(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Affectation WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return MapEntity(reader);
                }
            }

            return null;
        }

        public int Create(AffectationModel affectation)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"INSERT INTO Affectation 
                      (DateTransport, IdEmploye, IdAdresse, IdTypeTransport, IdSite, IdVehicule, IdHeureTransport, 
                       EstValidee, Commentaire, DateCreation, IdType, EstArchive) 
                      OUTPUT INSERTED.Id
                      VALUES 
                      (@DateTransport, @IdEmploye, @IdAdresse, @IdTypeTransport, @IdSite, @IdVehicule, @IdHeureTransport, 
                       @EstValidee, @Commentaire, @DateCreation, @IdType, @EstArchive)",
                    conn);

                AddParameter(cmd, "@DateTransport", affectation.DateTransport);
                AddParameter(cmd, "@IdEmploye", affectation.IdEmploye);
                AddParameter(cmd, "@IdAdresse", affectation.IdAdresse);
                AddParameter(cmd, "@IdTypeTransport", affectation.IdTypeTransport);
                AddParameter(cmd, "@IdSite", affectation.IdSite);
                AddParameter(cmd, "@IdVehicule", affectation.IdVehicule);
                AddParameter(cmd, "@IdHeureTransport", affectation.IdHeureTransport);
                AddParameter(cmd, "@EstValidee", affectation.EstValidee);
                AddParameter(cmd, "@Commentaire", affectation.Commentaire);
                AddParameter(cmd, "@DateCreation", affectation.DateCreation);
                AddParameter(cmd, "@IdType", affectation.IdType);
                AddParameter(cmd, "@EstArchive", affectation.EstArchive);

                return (int)cmd.ExecuteScalar();
            }
        }

        public void Update(AffectationModel affectation)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"UPDATE Affectation 
                      SET DateTransport = @DateTransport,
                          IdEmploye = @IdEmploye,
                          IdAdresse = @IdAdresse,
                          IdTypeTransport = @IdTypeTransport,
                          IdSite = @IdSite,
                          IdVehicule = @IdVehicule,
                          IdHeureTransport = @IdHeureTransport,
                          EstValidee = @EstValidee,
                          Commentaire = @Commentaire,
                          DateValidation = @DateValidation,
                          IdType = @IdType
                      WHERE Id = @Id",
                    conn);

                AddParameter(cmd, "@Id", affectation.Id);
                AddParameter(cmd, "@DateTransport", affectation.DateTransport);
                AddParameter(cmd, "@IdEmploye", affectation.IdEmploye);
                AddParameter(cmd, "@IdAdresse", affectation.IdAdresse);
                AddParameter(cmd, "@IdTypeTransport", affectation.IdTypeTransport);
                AddParameter(cmd, "@IdSite", affectation.IdSite);
                AddParameter(cmd, "@IdVehicule", affectation.IdVehicule);
                AddParameter(cmd, "@IdHeureTransport", affectation.IdHeureTransport);
                AddParameter(cmd, "@EstValidee", affectation.EstValidee);
                AddParameter(cmd, "@Commentaire", affectation.Commentaire);
                AddParameter(cmd, "@DateValidation", affectation.DateValidation);
                AddParameter(cmd, "@IdType", affectation.IdType);

                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM Affectation WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public List<AffectationModel> GetByDate(DateTime date)
        {
            var affectations = new List<AffectationModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT * FROM Affectation WHERE DateTransport = @DateTransport ORDER BY IdHeureTransport", 
                    conn);
                AddParameter(cmd, "@DateTransport", date.Date);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    affectations.Add(MapEntity(reader));
                }
            }

            return affectations;
        }

        public List<AffectationModel> GetByEmploye(int idEmploye)
        {
            var affectations = new List<AffectationModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT * FROM Affectation WHERE IdEmploye = @IdEmploye ORDER BY DateCreation DESC", 
                    conn);
                AddParameter(cmd, "@IdEmploye", idEmploye);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    affectations.Add(MapEntity(reader));
                }
            }

            return affectations;
        }

        public List<AffectationModel> GetByVehicule(int idVehicule)
        {
            var affectations = new List<AffectationModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT * FROM Affectation WHERE IdVehicule = @IdVehicule ORDER BY DateCreation DESC", 
                    conn);
                AddParameter(cmd, "@IdVehicule", idVehicule);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    affectations.Add(MapEntity(reader));
                }
            }

            return affectations;
        }

        public List<AffectationModel> GetEnAttente()
        {
            var affectations = new List<AffectationModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"SELECT * FROM Affectation 
                      WHERE EstValidee IS NULL 
                      AND EstArchive = 0 
                      ORDER BY DateCreation DESC", 
                    conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    affectations.Add(MapEntity(reader));
                }
            }

            return affectations;
        }

        public List<AffectationModel> GetValidees()
        {
            var affectations = new List<AffectationModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"SELECT * FROM Affectation 
                      WHERE EstValidee = 1 
                      AND EstArchive = 0 
                      ORDER BY DateCreation DESC", 
                    conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    affectations.Add(MapEntity(reader));
                }
            }

            return affectations;
        }

        public List<AffectationModel> GetValidatedAffectationsToday()
        {
            var affectations = new List<AffectationModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"SELECT
                        a.*,
                        v.Id AS Vehicule_Id,
                        v.Matricule AS Vehicule_Matricule,
                        v.NombrePlaces AS Vehicule_NombrePlaces,
                        s.Id AS Site_Id,
                        s.Nom AS Site_Nom,
                        ht.Id AS HeureTransport_Id,
                        ht.Heure AS HeureTransport_Heure,
                        tt.Id AS TypeTransport_Id,
                        tt.Libelle AS TypeTransport_Libelle,
                        e.Id AS Employe_Id,
                        e.Nom AS Employe_Nom,
                        e.Prenom AS Employe_Prenom,
                        e.Matricule AS Employe_Matricule,
                        ae.Id AS Adresse_Id,
                        ae.Adresse AS Adresse_Libelle,
                        ae.Latitude AS Adresse_Latitude,
                        ae.Longitude AS Adresse_Longitude,
                        s.Latitude AS Site_Latitude,
                        s.Longitude AS Site_Longitude
                      FROM Affectation a
                      LEFT JOIN Vehicule v ON v.Id = a.IdVehicule
                      INNER JOIN Site s ON s.Id = a.IdSite
                      INNER JOIN HeureTransport ht ON ht.Id = a.IdHeureTransport
                      INNER JOIN TypeTransport tt ON tt.Id = a.IdTypeTransport
                      INNER JOIN Employe e ON e.Id = a.IdEmploye
                      INNER JOIN AdresseEmploye ae ON ae.Id = a.IdAdresse
                      WHERE a.DateTransport = @DateTransport
                      AND a.EstValidee = 1
                      AND a.EstArchive = 0
                      ORDER BY v.Matricule, ht.Heure",
                    conn);
                AddParameter(cmd, "@DateTransport", DateTime.Today);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var affectation = MapEntity(reader);

                    affectation.Vehicule = reader["Vehicule_Id"] == DBNull.Value
                        ? null
                        : new VehiculeModel
                        {
                            Id = (int)reader["Vehicule_Id"],
                            Matricule = reader["Vehicule_Matricule"]?.ToString(),
                            NombrePlaces = reader["Vehicule_NombrePlaces"] == DBNull.Value ? null : (int?)reader["Vehicule_NombrePlaces"]
                        };

                    affectation.Site = new SiteModel
                    {
                        Id = (int)reader["Site_Id"],
                        Nom = reader["Site_Nom"]?.ToString(),
                        Latitude = reader["Site_Latitude"] == DBNull.Value ? null : (decimal?)reader["Site_Latitude"],
                        Longitude = reader["Site_Longitude"] == DBNull.Value ? null : (decimal?)reader["Site_Longitude"]
                    };

                    affectation.HeureTransport = new HeureTransportModel
                    {
                        Id = (int)reader["HeureTransport_Id"],
                        Heure = reader["HeureTransport_Heure"] == DBNull.Value ? null : (TimeSpan?)reader["HeureTransport_Heure"]
                    };

                    affectation.TypeTransport = new TypeTransportModel
                    {
                        Id = (int)reader["TypeTransport_Id"],
                        Libelle = reader["TypeTransport_Libelle"]?.ToString()
                    };

                    affectation.Employe = new EmployeModel
                    {
                        Id = (int)reader["Employe_Id"],
                        Nom = reader["Employe_Nom"]?.ToString() ?? string.Empty,
                        Prenom = reader["Employe_Prenom"]?.ToString() ?? string.Empty,
                        Matricule = reader["Employe_Matricule"]?.ToString()
                    };

                    affectation.Adresse = new AdresseEmployeModel
                    {
                        Id = (int)reader["Adresse_Id"],
                        Adresse = reader["Adresse_Libelle"]?.ToString(),
                        Latitude = reader["Adresse_Latitude"] == DBNull.Value ? null : (decimal?)reader["Adresse_Latitude"],
                        Longitude = reader["Adresse_Longitude"] == DBNull.Value ? null : (decimal?)reader["Adresse_Longitude"],
                        IdEmploye = affectation.IdEmploye
                    };

                    affectations.Add(affectation);
                }
            }

            return affectations;
        }

        public List<AffectationModel> GetRejetees()
        {
            var affectations = new List<AffectationModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"SELECT * FROM Affectation 
                      WHERE EstValidee = 0 
                      AND EstArchive = 0 
                      ORDER BY DateCreation DESC", 
                    conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    affectations.Add(MapEntity(reader));
                }
            }

            return affectations;
        }

        public List<AffectationModel> GetNonArchivees()
        {
            var affectations = new List<AffectationModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT * FROM Affectation WHERE EstArchive = 0 ORDER BY DateCreation DESC", 
                    conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    affectations.Add(MapEntity(reader));
                }
            }

            return affectations;
        }

        public List<AffectationModel> GetArchivees()
        {
            var affectations = new List<AffectationModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT * FROM Affectation WHERE EstArchive = 1 ORDER BY DateCreation DESC", 
                    conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    affectations.Add(MapEntity(reader));
                }
            }

            return affectations;
        }

        public void Valider(int id, string? commentaire = null)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"UPDATE Affectation 
                      SET EstValidee = 1, 
                          DateValidation = GETDATE(),
                          Commentaire = @Commentaire
                      WHERE Id = @Id",
                    conn);

                AddParameter(cmd, "@Id", id);
                AddParameter(cmd, "@Commentaire", commentaire);

                cmd.ExecuteNonQuery();
            }
        }

        public void Rejeter(int id, string? commentaire = null)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"UPDATE Affectation 
                      SET EstValidee = 0, 
                          DateValidation = GETDATE(),
                          Commentaire = @Commentaire
                      WHERE Id = @Id",
                    conn);

                AddParameter(cmd, "@Id", id);
                AddParameter(cmd, "@Commentaire", commentaire);
    
                cmd.ExecuteNonQuery();
            }
        }

        public void Archiver(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE Affectation SET EstArchive = 1 WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public void ArchiverDatesPassees()
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"UPDATE Affectation
                      SET EstArchive = 1
                      WHERE DateTransport < CAST(GETDATE() AS DATE)
                      AND EstArchive = 0",
                    conn);
                cmd.ExecuteNonQuery();
            }
        }

        public AffectationModel GetByEmployeAndDate(int idEmploye, DateTime date, int idTypeTransport)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"SELECT * FROM Affectation 
                      WHERE IdEmploye = @IdEmploye 
                      AND DateTransport = @DateTransport 
                      AND IdTypeTransport = @IdTypeTransport
                      AND EstArchive = 0", 
                    conn);
                AddParameter(cmd, "@IdEmploye", idEmploye);
                AddParameter(cmd, "@DateTransport", date.Date);
                AddParameter(cmd, "@IdTypeTransport", idTypeTransport);
                
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return MapEntity(reader);
                }
            }

            return null;
        }

        public List<AffectationModel> GetByDateAndTypeTransport(DateTime date, int idTypeTransport)
        {
            var affectations = new List<AffectationModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"SELECT * FROM Affectation 
                      WHERE DateTransport = @DateTransport 
                      AND IdTypeTransport = @IdTypeTransport 
                      ORDER BY IdHeureTransport", 
                    conn);
                AddParameter(cmd, "@DateTransport", date.Date);
                AddParameter(cmd, "@IdTypeTransport", idTypeTransport);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    affectations.Add(MapEntity(reader));
                }
            }

            return affectations;
        }

        public List<AffectationModel> GetByDateAndHeureAndSiteAndType(DateTime date, int idHeureTransport, int idSite, int idTypeTransport)
        {
            var affectations = new List<AffectationModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"SELECT * FROM Affectation 
                      WHERE DateTransport = @DateTransport 
                      AND IdHeureTransport = @IdHeureTransport
                      AND IdSite = @IdSite
                      AND IdTypeTransport = @IdTypeTransport
                      AND EstArchive = 0", 
                    conn);
                AddParameter(cmd, "@DateTransport", date.Date);
                AddParameter(cmd, "@IdHeureTransport", idHeureTransport);
                AddParameter(cmd, "@IdSite", idSite);
                AddParameter(cmd, "@IdTypeTransport", idTypeTransport);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    affectations.Add(MapEntity(reader));
                }
            }

            return affectations;
        }

        public int CountByVehiculeAndDateAndHeure(int idVehicule, DateTime date, int idHeureTransport)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"SELECT COUNT(*) FROM Affectation 
                      WHERE IdVehicule = @IdVehicule 
                      AND DateTransport = @DateTransport 
                      AND IdHeureTransport = @IdHeureTransport
                      AND EstArchive = 0",
                    conn);
                AddParameter(cmd, "@IdVehicule", idVehicule);
                AddParameter(cmd, "@DateTransport", date.Date);
                AddParameter(cmd, "@IdHeureTransport", idHeureTransport);
                return (int)cmd.ExecuteScalar();
            }
        }

        public void UpdateVehicule(int id, int? idVehicule)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "UPDATE Affectation SET IdVehicule = @IdVehicule WHERE Id = @Id",
                    conn);
                AddParameter(cmd, "@Id", id);
                AddParameter(cmd, "@IdVehicule", idVehicule);
                cmd.ExecuteNonQuery();
            }
        }

        public override void Activate(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE Affectation SET EstArchive = 0 WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public override void Deactivate(int id)
        {
            Archiver(id);
        }
    }
}
