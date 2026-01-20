namespace GestionTransport.FrontOffice.Models.Transport;

public class TypeTransportModel
{
    public int Id { get; set; }

    public string? Libelle { get; set; } // Aller, Retour

    public bool Actif { get; set; } = true;

    public bool EstAller() => Libelle?.ToLower().Contains("aller") ?? false;

    public bool EstRetour() => Libelle?.ToLower().Contains("retour") ?? false;
}