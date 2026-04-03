namespace GestionTransport.FrontOffice.Models.Auth;

public class AuthentificationModel
{
    public int Id { get; set; }
    public int IdEmploye { get; set; }
    public string Matricule { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string MotDePasse { get; set; } = string.Empty;
    public string Role { get; set; } = "EMPLOYE";
    public bool Actif { get; set; }
}

