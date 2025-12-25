using System.Data.SqlClient;
using GestionTransport.FrontOffice.Models;
using GestionTransport.FrontOffice.Services;
using Microsoft.Data.SqlClient;

namespace GestionTransport.FrontOffice.Repositories
{
    public interface ICompteRepository
    {
        List<EmployeModel> GetAll();
        EmployeModel GetById(int id);
        void Create(EmployeModel employe);
        void Update(EmployeModel employe);
        void Delete(int id);
        void UpdateAdresse(int employeId, string adresseDepart, string adresseArrivee, double? lat, double? lng);
    }

    public class CompteRepository : ICompteRepository
    {
        private readonly IDatabaseService _dbService;

        public CompteRepository(IDatabaseService dbService)
        {
            _dbService = dbService;
        }

        public List<EmployeModel> GetAll()
        {
            List<EmployeModel> employes = new List<EmployeModel>();

            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Employe", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    employes.Add(new EmployeModel
                    {
                        Id = (int)reader["Id"],
                        Nom = reader["Nom"].ToString(),
                        Prenom = reader["Prenom"].ToString(),
                        AdresseDepart = reader["AdresseDepart"].ToString(),
                        AdresseArrivee = reader["AdresseArrivee"].ToString(),
                        Telephone = reader["Telephone"]?.ToString()
                    });
                }
            }

            return employes;
        }

        public EmployeModel GetById(int id)
        {
            EmployeModel employe = null;

            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Employe WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    employe = new EmployeModel
                    {
                        Id = (int)reader["Id"],
                        Nom = reader["Nom"].ToString(),
                        Prenom = reader["Prenom"].ToString(),
                        AdresseDepart = reader["AdresseDepart"].ToString(),
                        AdresseArrivee = reader["AdresseArrivee"].ToString(),
                        Telephone = reader["Telephone"]?.ToString()
                    };
                }
            }

            return employe;
        }

        public void Create(EmployeModel employe)
        {
            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO Employe (Nom, Prenom, AdresseDepart, AdresseArrivee, Telephone) 
                      VALUES (@Nom, @Prenom, @AdresseDepart, @AdresseArrivee, @Telephone)",
                    conn);

                cmd.Parameters.AddWithValue("@Nom", employe.Nom);
                cmd.Parameters.AddWithValue("@Prenom", employe.Prenom);
                cmd.Parameters.AddWithValue("@AdresseDepart", employe.AdresseDepart);
                cmd.Parameters.AddWithValue("@AdresseArrivee", employe.AdresseArrivee);
                cmd.Parameters.AddWithValue("@Telephone", employe.Telephone ?? (object)DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        public void Update(EmployeModel employe)
        {
            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    @"UPDATE Employe 
                      SET Nom = @Nom, 
                          Prenom = @Prenom, 
                          AdresseDepart = @AdresseDepart, 
                          AdresseArrivee = @AdresseArrivee,
                          Telephone = @Telephone 
                      WHERE Id = @Id",
                    conn);

                cmd.Parameters.AddWithValue("@Id", employe.Id);
                cmd.Parameters.AddWithValue("@Nom", employe.Nom);
                cmd.Parameters.AddWithValue("@Prenom", employe.Prenom);
                cmd.Parameters.AddWithValue("@AdresseDepart", employe.AdresseDepart);
                cmd.Parameters.AddWithValue("@AdresseArrivee", employe.AdresseArrivee);
                cmd.Parameters.AddWithValue("@Telephone", employe.Telephone ?? (object)DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Employe WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateAdresse(int employeId, string adresseDepart, string adresseArrivee, double? lat, double? lng)
        {
            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                
                // Si tu veux stocker lat/lng aussi, il faudra ajouter ces colonnes à la table
                // Pour l'instant on update juste les adresses
                SqlCommand cmd = new SqlCommand(
                    @"UPDATE Employe 
                      SET AdresseDepart = @AdresseDepart, 
                          AdresseArrivee = @AdresseArrivee
                      WHERE Id = @Id",
                    conn);

                cmd.Parameters.AddWithValue("@Id", employeId);
                cmd.Parameters.AddWithValue("@AdresseDepart", adresseDepart);
                cmd.Parameters.AddWithValue("@AdresseArrivee", adresseArrivee);

                cmd.ExecuteNonQuery();
            }
        }
    }
}