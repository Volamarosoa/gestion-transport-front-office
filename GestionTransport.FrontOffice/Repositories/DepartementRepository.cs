using GestionTransport.FrontOffice.Models;
using GestionTransport.FrontOffice.Repositories.Interfaces;
using GestionTransport.FrontOffice.Services;
using Microsoft.Data.SqlClient;

namespace GestionTransport.FrontOffice.Repositories
{
    public class DepartementRepository : BaseRepository<DepartementModel>, IDepartementRepository
    {
        protected override string TableName => "Departement";

        public DepartementRepository(IDatabaseService dbService) : base(dbService) { }

        protected override DepartementModel MapEntity(SqlDataReader reader)
        {
            return new DepartementModel
            {
                Id = (int)reader["Id"],
                Nom = reader["Nom"].ToString(),
                Description = reader["Description"]?.ToString(),
                Actif = (bool)reader["Actif"],
                DateInsertion = (DateTime)reader["DateInsertion"],
                DateDesactivation = reader["DateDesactivation"] == DBNull.Value 
                    ? null 
                    : (DateTime?)reader["DateDesactivation"]
            };
        }

        public List<DepartementModel> GetAll()
        {
            var departements = new List<DepartementModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Departement ORDER BY Nom", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    departements.Add(MapEntity(reader));
                }
            }

            return departements;
        }

        public DepartementModel GetById(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Departement WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return MapEntity(reader);
                }
            }

            return null;
        }

        public int Create(DepartementModel departement)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"INSERT INTO Departement (Nom, Description, Actif, DateInsertion) 
                      OUTPUT INSERTED.Id
                      VALUES (@Nom, @Description, @Actif, @DateInsertion)",
                    conn);

                AddParameter(cmd, "@Nom", departement.Nom);
                AddParameter(cmd, "@Description", departement.Description);
                AddParameter(cmd, "@Actif", departement.Actif);
                AddParameter(cmd, "@DateInsertion", departement.DateInsertion);

                return (int)cmd.ExecuteScalar();
            }
        }

        public void Update(DepartementModel departement)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"UPDATE Departement 
                      SET Nom = @Nom, 
                          Description = @Description
                      WHERE Id = @Id",
                    conn);

                AddParameter(cmd, "@Id", departement.Id);
                AddParameter(cmd, "@Nom", departement.Nom);
                AddParameter(cmd, "@Description", departement.Description);

                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM Departement WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public List<DepartementModel> GetActifs()
        {
            var departements = new List<DepartementModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Departement WHERE Actif = 1 ORDER BY Nom", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    departements.Add(MapEntity(reader));
                }
            }

            return departements;
        }

        public DepartementModel GetByNom(string nom)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Departement WHERE Nom = @Nom", conn);
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