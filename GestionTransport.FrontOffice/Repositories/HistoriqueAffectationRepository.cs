using GestionTransport.FrontOffice.Models.Affectation;
using GestionTransport.FrontOffice.Repositories.Interfaces;
using GestionTransport.FrontOffice.Services;
using Microsoft.Data.SqlClient;

namespace GestionTransport.FrontOffice.Repositories
{
    public class HistoriqueAffectationRepository : IHistoriqueAffectationRepository
    {
        private readonly IDatabaseService _dbService;

        public HistoriqueAffectationRepository(IDatabaseService dbService)
        {
            _dbService = dbService;
        }

        private SqlConnection GetConnection() => _dbService.GetConnection();

        private void AddParameter(SqlCommand cmd, string name, object value)
        {
            cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
        }

        private HistoriqueAffectationModel MapEntity(SqlDataReader reader)
        {
            return new HistoriqueAffectationModel
            {
                IdHistorique = (int)reader["IdHistorique"],
                IdAffectation = (int)reader["IdAffectation"],
                Date = reader["Date"] == DBNull.Value ? null : (DateTime?)reader["Date"],
                IdEmploye = (int)reader["IdEmploye"],
                IdAdresse = (int)reader["IdAdresse"],
                IdTypeTransport = (int)reader["IdTypeTransport"],
                IdSite = (int)reader["IdSite"],
                IdVehicule = reader["IdVehicule"] == DBNull.Value ? null : (int?)reader["IdVehicule"],
                IdHeureTransport = (int)reader["IdHeureTransport"],
                EstValidee = reader["EstValidee"] == DBNull.Value ? null : (bool?)reader["EstValidee"],
                Commentaire = reader["Commentaire"]?.ToString(),
                DateCreation = (DateTime)reader["DateCreation"],
                DateValidation = reader["DateValidation"] == DBNull.Value ? null : (DateTime?)reader["DateValidation"],
                IdType = (int)reader["IdType"],
                DateModification = (DateTime)reader["DateModification"]
            };
        }

        public List<HistoriqueAffectationModel> GetByAffectation(int idAffectation)
        {
            var historiques = new List<HistoriqueAffectationModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT * FROM HistoriqueAffectation WHERE IdAffectation = @IdAffectation ORDER BY DateModification DESC",
                    conn);
                AddParameter(cmd, "@IdAffectation", idAffectation);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    historiques.Add(MapEntity(reader));
                }
            }

            return historiques;
        }

        public int Create(HistoriqueAffectationModel historique)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"INSERT INTO HistoriqueAffectation 
                      (IdAffectation, [Date], IdEmploye, IdAdresse, IdTypeTransport, IdSite, IdVehicule, IdHeureTransport,
                       EstValidee, Commentaire, DateCreation, DateValidation, IdType, DateModification)
                      OUTPUT INSERTED.IdHistorique
                      VALUES 
                      (@IdAffectation, @Date, @IdEmploye, @IdAdresse, @IdTypeTransport, @IdSite, @IdVehicule, @IdHeureTransport,
                       @EstValidee, @Commentaire, @DateCreation, @DateValidation, @IdType, @DateModification)",
                    conn);

                AddParameter(cmd, "@IdAffectation", historique.IdAffectation);
                AddParameter(cmd, "@Date", historique.Date);
                AddParameter(cmd, "@IdEmploye", historique.IdEmploye);
                AddParameter(cmd, "@IdAdresse", historique.IdAdresse);
                AddParameter(cmd, "@IdTypeTransport", historique.IdTypeTransport);
                AddParameter(cmd, "@IdSite", historique.IdSite);
                AddParameter(cmd, "@IdVehicule", historique.IdVehicule);
                AddParameter(cmd, "@IdHeureTransport", historique.IdHeureTransport);
                AddParameter(cmd, "@EstValidee", historique.EstValidee);
                AddParameter(cmd, "@Commentaire", historique.Commentaire);
                AddParameter(cmd, "@DateCreation", historique.DateCreation);
                AddParameter(cmd, "@DateValidation", historique.DateValidation);
                AddParameter(cmd, "@IdType", historique.IdType);
                AddParameter(cmd, "@DateModification", historique.DateModification);

                return (int)cmd.ExecuteScalar();
            }
        }
    }
}
