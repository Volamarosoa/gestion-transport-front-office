namespace GestionTransport.FrontOffice.Models;

public class SiteModel
{
    public int Id { get; set; }

    public string? Nom { get; set; }

    public string? Adresse { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public bool Actif { get; set; } = true;

    public DateTime DateInsertion { get; set; } = DateTime.Now;

    public DateTime? DateDesactivation { get; set; }

    public void Desactiver()
    {
        Actif = false;
        DateDesactivation = DateTime.Now;
    }

    public void Activer()
    {
        Actif = true;
        DateDesactivation = null;
    }

    public bool EstActif() => Actif && DateDesactivation == null;

    public bool ADesCoordonnees() => Latitude.HasValue && Longitude.HasValue;

    public string GetTypeSite()
    {
        if (string.IsNullOrEmpty(Nom)) return "Non défini";
        
        return Nom.ToLower() switch
        {
            var n when n.Contains("siège") || n.Contains("siege") => "Siège",
            var n when n.Contains("usine") => "Usine",
            var n when n.Contains("agence") => "Agence",
            var n when n.Contains("entrepôt") || n.Contains("entrepot") => "Entrepôt",
            _ => Nom
        };
    }
}