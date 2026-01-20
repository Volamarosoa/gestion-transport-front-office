namespace GestionTransport.FrontOffice.Models.Employe;

public class AdresseEmployeModel
{
    public int Id { get; set; }

    public int IdEmploye { get; set; }

    public string? Adresse { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public bool EstPrincipale { get; set; } = false;

    public bool Actif { get; set; } = true;

    public DateTime DateInsertion { get; set; } = DateTime.Now;

    public DateTime? DateDesactivation { get; set; }

    public EmployeModel? Employe { get; set; }

    public void DefinirCommePrincipale()
    {
        EstPrincipale = true;
        Actif = true;
    }

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
}