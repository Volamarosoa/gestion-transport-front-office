using GestionTransport.FrontOffice.Models;
using GestionTransport.FrontOffice.Repositories.Interfaces;
using GestionTransport.FrontOffice.Services;
using Microsoft.Data.SqlClient;

namespace GestionTransport.FrontOffice.Repositories
{
    public class VehiculeRepository : BaseRepository<VehiculeModel>, IVehiculeRepository
    {
        protected override string TableName => "Vehicule";

        public VehiculeRepository(IDatabaseService dbService) : base(dbService) { }

        protected override VehiculeModel MapEntity(SqlDataReader reader)
        {
            return new VehiculeModel
            {
                Id = (int)reader["Id"],
                Matricule = reader["Matricule"]?.ToString(),
                NombrePlaces = reader["NombrePlaces"] == DBNull.Value 
                    ? null 
                    : (int?)reader["NombrePlaces"],
                Actif = (bool)reader["Actif"],
                DateInsertion = (DateTime)reader["DateInsertion"],
                DateDesactivation = reader["DateDesactivation"] == DBNull.Value 
                    ? null 
                    : (DateTime?)reader["DateDesactivation"]
            };
        }

        public List<VehiculeModel> GetAll()
        {
            var vehicules = new List<VehiculeModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Vehicule ORDER BY Matricule", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    vehicules.Add(MapEntity(reader));
                }
            }

            return vehicules;
        }

        public VehiculeModel GetById(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Vehicule WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return MapEntity(reader);
                }
            }

            return null;
        }

        public int Create(VehiculeModel vehicule)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"INSERT INTO Vehicule (Matricule, NombrePlaces, Actif, DateInsertion) 
                      OUTPUT INSERTED.Id
                      VALUES (@Matricule, @NombrePlaces, @Actif, @DateInsertion)",
                    conn);

                AddParameter(cmd, "@Matricule", vehicule.Matricule);
                AddParameter(cmd, "@NombrePlaces", vehicule.NombrePlaces);
                AddParameter(cmd, "@Actif", vehicule.Actif);
                AddParameter(cmd, "@DateInsertion", vehicule.DateInsertion);

                return (int)cmd.ExecuteScalar();
            }
        }

        public void Update(VehiculeModel vehicule)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"UPDATE Vehicule 
                      SET Matricule = @Matricule, 
                          NombrePlaces = @NombrePlaces
                      WHERE Id = @Id",
                    conn);

                AddParameter(cmd, "@Id", vehicule.Id);
                AddParameter(cmd, "@Matricule", vehicule.Matricule);
                AddParameter(cmd, "@NombrePlaces", vehicule.NombrePlaces);

                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM Vehicule WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public List<VehiculeModel> GetActifs()
        {
            var vehicules = new List<VehiculeModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Vehicule WHERE Actif = 1 ORDER BY Matricule", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    vehicules.Add(MapEntity(reader));
                }
            }

            return vehicules;
        }

        public List<VehiculeModel> GetDisponibles(int placesRequises)
        {
            var vehicules = new List<VehiculeModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"SELECT * FROM Vehicule 
                      WHERE Actif = 1 
                      AND NombrePlaces >= @PlacesRequises 
                      ORDER BY NombrePlaces, Matricule", 
                    conn);
                AddParameter(cmd, "@PlacesRequises", placesRequises);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    vehicules.Add(MapEntity(reader));
                }
            }

            return vehicules;
        }

        public VehiculeModel GetByMatricule(string matricule)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Vehicule WHERE Matricule = @Matricule", conn);
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