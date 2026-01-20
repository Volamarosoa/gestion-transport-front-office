namespace GestionTransport.FrontOffice.Models.Transport;

public class HeureTransportModel
{
    public int Id { get; set; }

    public TimeSpan? Heure { get; set; }

    public string? Libelle { get; set; } // Matin, Soir, Nuit

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

    public string GetPeriodeJournee()
    {
        if (!Heure.HasValue) return "Non définie";

        return Heure.Value.Hours switch
        {
            >= 5 and < 12 => "Matin",
            >= 12 and < 18 => "Après-midi",
            >= 18 and < 22 => "Soir",
            _ => "Nuit"
        };
    }

    public string FormatHeure() => Heure?.ToString(@"hh\:mm") ?? "--:--";
}