using GestionTransport.FrontOffice.Models.Affectation;
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
                IdDate = (int)reader["IdDate"],
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
                      (IdDate, IdEmploye, IdAdresse, IdTypeTransport, IdSite, IdVehicule, IdHeureTransport, 
                       EstValidee, Commentaire, DateCreation, IdType, EstArchive) 
                      OUTPUT INSERTED.Id
                      VALUES 
                      (@IdDate, @IdEmploye, @IdAdresse, @IdTypeTransport, @IdSite, @IdVehicule, @IdHeureTransport, 
                       @EstValidee, @Commentaire, @DateCreation, @IdType, @EstArchive)",
                    conn);

                AddParameter(cmd, "@IdDate", affectation.IdDate);
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
                      SET IdDate = @IdDate,
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
                AddParameter(cmd, "@IdDate", affectation.IdDate);
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

        public List<AffectationModel> GetByDate(int idDate)
        {
            var affectations = new List<AffectationModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT * FROM Affectation WHERE IdDate = @IdDate ORDER BY IdHeureTransport", 
                    conn);
                AddParameter(cmd, "@IdDate", idDate);
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

        public void ArchiverParDate(int idDate)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "UPDATE Affectation SET EstArchive = 1 WHERE IdDate = @IdDate AND EstArchive = 0", 
                    conn);
                AddParameter(cmd, "@IdDate", idDate);
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
WHERE IdDate IN (
SELECT Id FROM DateTransport
WHERE DateJour < CAST(GETDATE() AS DATE)
)
AND EstArchive = 0",
conn);
cmd.ExecuteNonQuery();
}
}
    public AffectationModel GetByEmployeAndDate(int idEmploye, int idDate, int idTypeTransport)
    {
        using (var conn = GetConnection())
        {
            conn.Open();
            var cmd = new SqlCommand(
                @"SELECT * FROM Affectation 
                  WHERE IdEmploye = @IdEmploye 
                  AND IdDate = @IdDate 
                  AND IdTypeTransport = @IdTypeTransport", 
                conn);
            AddParameter(cmd, "@IdEmploye", idEmploye);
            AddParameter(cmd, "@IdDate", idDate);
            AddParameter(cmd, "@IdTypeTransport", idTypeTransport);
            
            var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return MapEntity(reader);
            }
        }

        return null;
    }

    public List<AffectationModel> GetByDateAndTypeTransport(int idDate, int idTypeTransport)
    {
        var affectations = new List<AffectationModel>();

        using (var conn = GetConnection())
        {
            conn.Open();
            var cmd = new SqlCommand(
                @"SELECT * FROM Affectation 
                  WHERE IdDate = @IdDate 
                  AND IdTypeTransport = @IdTypeTransport 
                  ORDER BY IdHeureTransport", 
                conn);
            AddParameter(cmd, "@IdDate", idDate);
            AddParameter(cmd, "@IdTypeTransport", idTypeTransport);
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                affectations.Add(MapEntity(reader));
            }
        }

        return affectations;
    }

    // Override - Cette table n'a pas de colonne Actif standard
    public override void Activate(int id)
    {
        // Pas applicable pour Affectation
        throw new NotImplementedException("Affectation n'a pas de colonne Actif");
    }

    public override void Deactivate(int id)
    {
        // Utiliser Archiver à la place
        Archiver(id);
    }
}
}