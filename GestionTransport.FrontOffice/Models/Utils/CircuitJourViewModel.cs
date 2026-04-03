namespace GestionTransport.FrontOffice.Models.Utils;

public class CircuitJourViewModel
{
    public DateTime Date { get; set; }
    public List<VehiculeCircuitViewModel> Vehicules { get; set; } = new();
}

public class VehiculeCircuitViewModel
{
    public int VehiculeId { get; set; }
    public string VehiculeLabel { get; set; } = string.Empty;
    public int? NombrePlaces { get; set; }
    public SitePointViewModel? Site { get; set; }
    public List<PersonneCircuitViewModel> Personnes { get; set; } = new();
}

public class SitePointViewModel
{
    public int SiteId { get; set; }
    public string? Nom { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}

public class PersonneCircuitViewModel
{
    public int EmployeId { get; set; }
    public string NomComplet { get; set; } = string.Empty;
    public string? Matricule { get; set; }
    public string? Adresse { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? Heure { get; set; }
    public string? TypeTransport { get; set; }
    public double? DistanceKmVersSite { get; set; }
}

