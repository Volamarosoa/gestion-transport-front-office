using GestionTransport.FrontOffice.Models;
using GestionTransport.FrontOffice.Repositories.Interfaces;
using GestionTransport.FrontOffice.Services;
using Microsoft.Data.SqlClient;

namespace GestionTransport.FrontOffice.Repositories
{
    public class DateTransportRepository : BaseRepository<DateTransportModel>, IDateTransportRepository
    {
        protected override string TableName => "DateTransport";

        public DateTransportRepository(IDatabaseService dbService) : base(dbService) { }

        protected override DateTransportModel MapEntity(SqlDataReader reader)
        {
            return new DateTransportModel
            {
                Id = (int)reader["Id"],
                DateJour = (DateTime)reader["DateJour"],
                Actif = (bool)reader["Actif"]
            };
        }

        public List<DateTransportModel> GetAll()
        {
            var dates = new List<DateTransportModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM DateTransport ORDER BY DateJour DESC", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    dates.Add(MapEntity(reader));
                }
            }

            return dates;
        }

        public DateTransportModel GetById(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM DateTransport WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return MapEntity(reader);
                }
            }

            return null;
        }

        public int Create(DateTransportModel dateTransport)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"INSERT INTO DateTransport (DateJour, Actif) 
                      OUTPUT INSERTED.Id
                      VALUES (@DateJour, @Actif)",
                    conn);

                AddParameter(cmd, "@DateJour", dateTransport.DateJour.Date);
                AddParameter(cmd, "@Actif", dateTransport.Actif);

                return (int)cmd.ExecuteScalar();
            }
        }

        public void Update(DateTransportModel dateTransport)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"UPDATE DateTransport 
                      SET DateJour = @DateJour,
                          Actif = @Actif
                      WHERE Id = @Id",
                    conn);

                AddParameter(cmd, "@Id", dateTransport.Id);
                AddParameter(cmd, "@DateJour", dateTransport.DateJour.Date);
                AddParameter(cmd, "@Actif", dateTransport.Actif);

                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM DateTransport WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public List<DateTransportModel> GetActifs()
        {
            var dates = new List<DateTransportModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT * FROM DateTransport WHERE Actif = 1 ORDER BY DateJour DESC", 
                    conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    dates.Add(MapEntity(reader));
                }
            }

            return dates;
        }

        public List<DateTransportModel> GetFutures()
        {
            var dates = new List<DateTransportModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"SELECT * FROM DateTransport 
                      WHERE DateJour >= CAST(GETDATE() AS DATE) 
                      AND Actif = 1 
                      ORDER BY DateJour", 
                    conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    dates.Add(MapEntity(reader));
                }
            }

            return dates;
        }

        public List<DateTransportModel> GetByPeriode(DateTime dateDebut, DateTime dateFin)
        {
            var dates = new List<DateTransportModel>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"SELECT * FROM DateTransport 
                      WHERE DateJour BETWEEN @DateDebut AND @DateFin 
                      ORDER BY DateJour", 
                    conn);
                AddParameter(cmd, "@DateDebut", dateDebut.Date);
                AddParameter(cmd, "@DateFin", dateFin.Date);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    dates.Add(MapEntity(reader));
                }
            }

            return dates;
        }

        public DateTransportModel GetByDate(DateTime date)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM DateTransport WHERE DateJour = @DateJour", conn);
                AddParameter(cmd, "@DateJour", date.Date);
                
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return MapEntity(reader);
                }
            }

            return null;
        }

        public DateTransportModel GetOrCreateByDate(DateTime date)
        {
            var existing = GetByDate(date);
            if (existing != null)
                return existing;

            var newDate = new DateTransportModel
            {
                DateJour = date.Date,
                Actif = true
            };

            newDate.Id = Create(newDate);
            return newDate;
        }

        // Override car cette table n'a pas DateDesactivation
        public override void Activate(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE DateTransport SET Actif = 1 WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public override void Deactivate(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE DateTransport SET Actif = 0 WHERE Id = @Id", conn);
                AddParameter(cmd, "@Id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}