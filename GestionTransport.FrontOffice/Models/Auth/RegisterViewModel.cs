namespace GestionTransport.FrontOffice.Models.Auth;

public class RegisterViewModel
{
    public string Matricule { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MotDePasse { get; set; } = string.Empty;
    public string ConfirmationMotDePasse { get; set; } = string.Empty;
}
