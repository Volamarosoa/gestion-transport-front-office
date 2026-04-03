using GestionTransport.FrontOffice.Models.Employe;
using GestionTransport.FrontOffice.Repositories.Interfaces;
using GestionTransport.FrontOffice.Services;
using Microsoft.Data.SqlClient;

namespace GestionTransport.FrontOffice.Repositories
{
    public class EmployeRepository : BaseRepository<EmployeModel>, IEmployeRepository
    {
        protected override string TableName => "Employe";

        public EmployeRepository(IDatabaseService dbService) : base(dbService) { }

        protected override EmployeModel MapEntity(SqlDataReader reader)
        {
            return new EmployeModel
            {
                Id = (int)reader["Id"],
                Nom = reader["Nom"].ToString(),
                Prenom = reader["Prenom"].ToString(),
                Matricule = reader["Matricule"]?.ToString(),
                Telephone = reader["Telephone"]?.ToString(),
                Email = reader["Email"] == DBNull.Value ? null : reader["Email"]?.ToString(),
                IdDepartement = (int)reader["IdDepartement"],
                Actif = (bool)reader["Actif"],
                EstBeneficiaire = (bool)reader["EstBeneficiaire"],
                DateInsertion = (DateTime)reader["DateInsertion"],
                DateDesactivation = reader["DateDesactivation"] == DBNull.Value 
                    ? null 
                    : (DateTime?)reader["DateDesactivation"]
            };
        }

        public List<EmployeModel> GetAll()
        {
            var employes = new List<EmployeModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Employe", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    employes.Add(MapEntity(reader));
                }
            }

            return employes;
        }

        public EmployeModel GetById(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Employe WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return MapEntity(reader);
                }
            }

            return null;
        }

        public int Create(EmployeModel employe)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"INSERT INTO Employe (Nom, Prenom, Matricule, Telephone, Email, IdDepartement, Actif, EstBeneficiaire, DateInsertion) 
                      OUTPUT INSERTED.Id
                      VALUES (@Nom, @Prenom, @Matricule, @Telephone, @Email, @IdDepartement, @Actif, @EstBeneficiaire, @DateInsertion)",
                    conn);

                AddParameter(cmd, "@Nom", employe.Nom);
                AddParameter(cmd, "@Prenom", employe.Prenom);
                AddParameter(cmd, "@Matricule", employe.Matricule);
                AddParameter(cmd, "@Telephone", employe.Telephone);
                AddParameter(cmd, "@Email", employe.Email);
                AddParameter(cmd, "@IdDepartement", employe.IdDepartement);
                AddParameter(cmd, "@Actif", employe.Actif);
                AddParameter(cmd, "@EstBeneficiaire", employe.EstBeneficiaire);
                AddParameter(cmd, "@DateInsertion", employe.DateInsertion);

                return (int)cmd.ExecuteScalar();
            }
        }

        public void Update(EmployeModel employe)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"UPDATE Employe 
                      SET Nom = @Nom, 
                          Prenom = @Prenom, 
                          Matricule = @Matricule,
                          Telephone = @Telephone,
                          Email = @Email,
                          IdDepartement = @IdDepartement,
                          EstBeneficiaire = @EstBeneficiaire
                      WHERE Id = @Id",
                    conn);

                AddParameter(cmd, "@Id", employe.Id);
                AddParameter(cmd, "@Nom", employe.Nom);
                AddParameter(cmd, "@Prenom", employe.Prenom);
                AddParameter(cmd, "@Matricule", employe.Matricule);
                AddParameter(cmd, "@Telephone", employe.Telephone);
                AddParameter(cmd, "@Email", employe.Email);
                AddParameter(cmd, "@IdDepartement", employe.IdDepartement);
                AddParameter(cmd, "@EstBeneficiaire", employe.EstBeneficiaire);

                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM Employe WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public List<EmployeModel> GetByDepartement(int idDepartement)
        {
            var employes = new List<EmployeModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Employe WHERE IdDepartement = @IdDepartement AND Actif = 1", conn);
                AddParameter(cmd, "@IdDepartement", idDepartement);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    employes.Add(MapEntity(reader));
                }
            }

            return employes;
        }

        public List<EmployeModel> GetActifs()
        {
            var employes = new List<EmployeModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Employe WHERE Actif = 1", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    employes.Add(MapEntity(reader));
                }
            }

            return employes;
        }

        public EmployeModel? GetByMatricule(string matricule)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Employe WHERE Matricule = @Matricule", conn);
                AddParameter(cmd, "@Matricule", matricule);
                
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return MapEntity(reader);
                }
            }

            return null;
        }

        public long CountByMatriculeStartingWith(string prefix)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM Employe WHERE Matricule LIKE @Prefix + '%'", conn);
                AddParameter(cmd, "@Prefix", prefix);
                return (int)cmd.ExecuteScalar();
            }
        }
    }
}
