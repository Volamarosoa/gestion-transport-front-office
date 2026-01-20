namespace GestionTransport.FrontOffice.Models;

public class VehiculeModel
{
    public int Id { get; set; }

    public string? Matricule { get; set; }

    public int? NombrePlaces { get; set; }

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

    public bool EstDisponible(int placesRequises) 
        => EstActif() && NombrePlaces.HasValue && NombrePlaces.Value >= placesRequises;
}