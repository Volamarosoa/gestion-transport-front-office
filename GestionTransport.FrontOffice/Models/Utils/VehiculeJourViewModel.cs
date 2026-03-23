namespace GestionTransport.FrontOffice.Models.Utils;

public class VehiculeJourViewModel
{
    public int? VehiculeId { get; set; }
    public string VehiculeLabel { get; set; } = string.Empty;
    public int? NombrePlaces { get; set; }
    public List<EmployeTransportItemViewModel> Employes { get; set; } = new();
}

public class EmployeTransportItemViewModel
{
    public string? NomComplet { get; set; }
    public string? Matricule { get; set; }
    public string? Adresse { get; set; }
    public string? Site { get; set; }
    public string? TypeTransport { get; set; }
    public string? Heure { get; set; }
    public string? Commentaire { get; set; }
}
