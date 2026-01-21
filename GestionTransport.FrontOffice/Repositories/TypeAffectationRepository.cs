using GestionTransport.FrontOffice.Models.Affectation;
using GestionTransport.FrontOffice.Repositories.Interfaces;
using GestionTransport.FrontOffice.Services;
using Microsoft.Data.SqlClient;

namespace GestionTransport.FrontOffice.Repositories
{
    public class TypeAffectationRepository : BaseRepository<TypeAffectationModel>, ITypeAffectationRepository
    {
        protected override string TableName => "TypeAffectation";

        public TypeAffectationRepository(IDatabaseService dbService) : base(dbService) { }

        protected override TypeAffectationModel MapEntity(SqlDataReader reader)
        {
            return new TypeAffectationModel
            {
                Id = (int)reader["Id"],
                Libelle = reader["Libelle"]?.ToString(),
                Actif = (bool)reader["Actif"]
            };
        }

        public List<TypeAffectationModel> GetAll()
        {
            var types = new List<TypeAffectationModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM TypeAffectation ORDER BY Libelle", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    types.Add(MapEntity(reader));
                }
            }

            return types;
        }

        public TypeAffectationModel GetById(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM TypeAffectation WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return MapEntity(reader);
                }
            }

            return null;
        }

        public int Create(TypeAffectationModel typeAffectation)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"INSERT INTO TypeAffectation (Libelle, Actif) 
                      OUTPUT INSERTED.Id
                      VALUES (@Libelle, @Actif)",
                    conn);

                AddParameter(cmd, "@Libelle", typeAffectation.Libelle);
                AddParameter(cmd, "@Actif", typeAffectation.Actif);

                return (int)cmd.ExecuteScalar();
            }
        }

        public void Update(TypeAffectationModel typeAffectation)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"UPDATE TypeAffectation 
                      SET Libelle = @Libelle,
                          Actif = @Actif
                      WHERE Id = @Id",
                    conn);

                AddParameter(cmd, "@Id", typeAffectation.Id);
                AddParameter(cmd, "@Libelle", typeAffectation.Libelle);
                AddParameter(cmd, "@Actif", typeAffectation.Actif);

                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM TypeAffectation WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public List<TypeAffectationModel> GetActifs()
        {
            var types = new List<TypeAffectationModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM TypeAffectation WHERE Actif = 1 ORDER BY Libelle", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    types.Add(MapEntity(reader));
                }
            }

            return types;
        }

        public TypeAffectationModel GetByLibelle(string libelle)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM TypeAffectation WHERE Libelle = @Libelle", conn);
                AddParameter(cmd, "@Libelle", libelle);
                
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return MapEntity(reader);
                }
            }

            return null;
        }

        // Override car cette table n'a pas DateDesactivation
        public override void Activate(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE TypeAffectation SET Actif = 1 WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public override void Deactivate(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE TypeAffectation SET Actif = 0 WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}