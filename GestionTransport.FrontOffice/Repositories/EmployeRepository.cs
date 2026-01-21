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
                IdDepartement = (int)reader["IdDepartement"],
                Actif = (bool)reader["Actif"],
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
                    @"INSERT INTO Employe (Nom, Prenom, Matricule, Telephone, IdDepartement, Actif, DateInsertion) 
                      OUTPUT INSERTED.Id
                      VALUES (@Nom, @Prenom, @Matricule, @Telephone, @IdDepartement, @Actif, @DateInsertion)",
                    conn);

                AddParameter(cmd, "@Nom", employe.Nom);
                AddParameter(cmd, "@Prenom", employe.Prenom);
                AddParameter(cmd, "@Matricule", employe.Matricule);
                AddParameter(cmd, "@Telephone", employe.Telephone);
                AddParameter(cmd, "@IdDepartement", employe.IdDepartement);
                AddParameter(cmd, "@Actif", employe.Actif);
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
                          IdDepartement = @IdDepartement
                      WHERE Id = @Id",
                    conn);

                AddParameter(cmd, "@Id", employe.Id);
                AddParameter(cmd, "@Nom", employe.Nom);
                AddParameter(cmd, "@Prenom", employe.Prenom);
                AddParameter(cmd, "@Matricule", employe.Matricule);
                AddParameter(cmd, "@Telephone", employe.Telephone);
                AddParameter(cmd, "@IdDepartement", employe.IdDepartement);

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
                var cmd = new SqlCommand("SELECT * FROM Employe WHERE IdDepartement = @IdDepartement", conn);
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

        public EmployeModel GetByMatricule(string matricule)
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
    }
}