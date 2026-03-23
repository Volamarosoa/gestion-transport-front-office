namespace GestionTransport.FrontOffice.Models.Auth;

public class LoginViewModel
{
    public string Matricule { get; set; } = string.Empty;
    public string MotDePasse { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}
