using GestionTransport.FrontOffice.Models.Auth;
using GestionTransport.FrontOffice.Repositories.Interfaces;
using GestionTransport.FrontOffice.Services;
using Microsoft.Data.SqlClient;

namespace GestionTransport.FrontOffice.Repositories;

public class AuthentificationRepository : IAuthentificationRepository
{
    private readonly IDatabaseService _dbService;

    public AuthentificationRepository(IDatabaseService dbService)
    {
        _dbService = dbService;
    }

    private SqlConnection GetConnection()
    {
        return _dbService.GetConnection();
    }

    public AuthentificationModel? GetByMatricule(string matricule)
    {
        using var conn = GetConnection();
        conn.Open();

        var cmd = new SqlCommand(@"
SELECT TOP 1
    a.Id,
    a.IdEmploye,
    e.Matricule,
    e.Email,
    a.MotDePasse,
    ISNULL(r.Libelle, 'EMPLOYE') AS Role,
    a.Actif
FROM Authentification a
INNER JOIN Employe e ON e.Id = a.IdEmploye
LEFT JOIN Role r ON r.Id = a.IdRole
WHERE e.Matricule = @Matricule", conn);

        cmd.Parameters.AddWithValue("@Matricule", matricule);

        using var reader = cmd.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new AuthentificationModel
        {
            Id = (int)reader["Id"],
            IdEmploye = (int)reader["IdEmploye"],
            Matricule = reader["Matricule"].ToString() ?? string.Empty,
            Email = reader["Email"] == DBNull.Value ? null : reader["Email"].ToString(),
            MotDePasse = reader["MotDePasse"].ToString() ?? string.Empty,
            Role = reader["Role"].ToString() ?? "EMPLOYE",
            Actif = (bool)reader["Actif"]
        };
    }

    public bool ExistsForEmploye(int idEmploye)
    {
        using var conn = GetConnection();
        conn.Open();

        var cmd = new SqlCommand("SELECT COUNT(1) FROM Authentification WHERE IdEmploye = @IdEmploye", conn);
        cmd.Parameters.AddWithValue("@IdEmploye", idEmploye);

        return (int)cmd.ExecuteScalar()! > 0;
    }

    public int Create(int idEmploye, string motDePasseHash, int idRole)
    {
        using var conn = GetConnection();
        conn.Open();

        var cmd = new SqlCommand(@"
INSERT INTO Authentification (IdEmploye, MotDePasse, IdRole, Actif, DateCreation)
OUTPUT INSERTED.Id
VALUES (@IdEmploye, @MotDePasse, @IdRole, 1, GETDATE())", conn);

        cmd.Parameters.AddWithValue("@IdEmploye", idEmploye);
        cmd.Parameters.AddWithValue("@MotDePasse", motDePasseHash);
        cmd.Parameters.AddWithValue("@IdRole", idRole);

        return (int)cmd.ExecuteScalar()!;
    }

    public int GetOrCreateRole(string libelleRole)
    {
        using var conn = GetConnection();
        conn.Open();

        var selectCmd = new SqlCommand("SELECT TOP 1 Id FROM Role WHERE UPPER(Libelle) = UPPER(@Libelle)", conn);
        selectCmd.Parameters.AddWithValue("@Libelle", libelleRole);

        var existing = selectCmd.ExecuteScalar();
        if (existing != null && existing != DBNull.Value)
        {
            return (int)existing;
        }

        var insertCmd = new SqlCommand(@"
INSERT INTO Role (Libelle)
OUTPUT INSERTED.Id
VALUES (@Libelle)", conn);
        insertCmd.Parameters.AddWithValue("@Libelle", libelleRole.ToUpperInvariant());

        return (int)insertCmd.ExecuteScalar()!;
    }

    public int? FindEmployeIdByMatriculeOrEmail(string? matricule, string? email)
    {
        using var conn = GetConnection();
        conn.Open();

        var cmd = new SqlCommand(@"
SELECT TOP 1 Id
FROM Employe
WHERE (@Matricule IS NULL OR Matricule = @Matricule)
  AND (@Email IS NULL OR Email = @Email)
  AND Actif = 1", conn);

        cmd.Parameters.AddWithValue("@Matricule", string.IsNullOrWhiteSpace(matricule) ? DBNull.Value : matricule.Trim());
        cmd.Parameters.AddWithValue("@Email", string.IsNullOrWhiteSpace(email) ? DBNull.Value : email.Trim());

        var result = cmd.ExecuteScalar();
        if (result == null || result == DBNull.Value)
        {
            return null;
        }

        return (int)result;
    }

    public void SaveResetToken(int idEmploye, string token, DateTime expirationUtc)
    {
        using var conn = GetConnection();
        conn.Open();

        var ensureColumnsCmd = new SqlCommand(@"
IF COL_LENGTH('Authentification', 'ResetToken') IS NULL
BEGIN
    ALTER TABLE Authentification ADD ResetToken NVARCHAR(200) NULL;
END

IF COL_LENGTH('Authentification', 'ResetTokenExpirationUtc') IS NULL
BEGIN
    ALTER TABLE Authentification ADD ResetTokenExpirationUtc DATETIME NULL;
END", conn);
        ensureColumnsCmd.ExecuteNonQuery();

        var cmd = new SqlCommand(@"
UPDATE Authentification
SET ResetToken = @Token,
    ResetTokenExpirationUtc = @ExpirationUtc
WHERE IdEmploye = @IdEmploye", conn);

        cmd.Parameters.AddWithValue("@IdEmploye", idEmploye);
        cmd.Parameters.AddWithValue("@Token", token);
        cmd.Parameters.AddWithValue("@ExpirationUtc", expirationUtc);

        cmd.ExecuteNonQuery();
    }
}


