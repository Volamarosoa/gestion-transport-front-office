using GestionTransport.FrontOffice.Models.Auth;

namespace GestionTransport.FrontOffice.Repositories.Interfaces;

public interface IAuthentificationRepository
{
    AuthentificationModel? GetByMatricule(string matricule);
    bool ExistsForEmploye(int idEmploye);
    int Create(int idEmploye, string motDePasseHash, int idRole);
    int GetOrCreateRole(string libelleRole);
    int? FindEmployeIdByMatriculeOrEmail(string? matricule, string? email);
    void SaveResetToken(int idEmploye, string token, DateTime expirationUtc);
}

