namespace GestionTransport.FrontOffice.Models.Affectation;

public class AffectationModel
{
    public int Id { get; set; }

    // Clés étrangères
    public int IdDate { get; set; }
    public int IdEmploye { get; set; }
    public int IdAdresse { get; set; }
    public int IdTypeTransport { get; set; }
    public int IdSite { get; set; }
    public int? IdVehicule { get; set; }
    public int IdHeureTransport { get; set; }
    public int IdType { get; set; }

    public bool? EstValidee { get; set; }
    public string? Commentaire { get; set; }

    public DateTime DateCreation { get; set; } = DateTime.Now;
    public DateTime? DateValidation { get; set; }

    public bool EstArchive { get; set; } = false;

    public DateTransportModel? DateTransport { get; set; }
    public EmployeModel? Employe { get; set; }
    public AdresseEmployeModel? Adresse { get; set; }
    public TypeTransportModel? TypeTransport { get; set; }
    public SiteModel? Site { get; set; }
    public VehiculeModel? Vehicule { get; set; }
    public HeureTransportModel? HeureTransport { get; set; }
    public TypeAffectationModel? TypeAffectation { get; set; }

    public void Valider(string? commentaire = null)
    {
        EstValidee = true;
        DateValidation = DateTime.Now;
        if (!string.IsNullOrEmpty(commentaire))
            Commentaire = commentaire;
    }

    public void Rejeter(string? commentaire = null)
    {
        EstValidee = false;
        DateValidation = DateTime.Now;
        if (!string.IsNullOrEmpty(commentaire))
            Commentaire = commentaire;
    }

    public void Archiver()
    {
        EstArchive = true;
    }

    public void Restaurer()
    {
        EstArchive = false;
    }

    // Vérification
    public bool EstEnAttente() => !EstValidee.HasValue;

    public bool EstActive() => !EstArchive && EstValidee == true;

    public bool PeutEtreModifiee() => !EstArchive && !EstValidee.HasValue;

    public bool EstAutomatique() => TypeAffectation?.EstAutomatique() ?? false;

    public bool EstManuelle() => TypeAffectation?.EstManuel() ?? false;

    public bool VehiculeAffecte() => IdVehicule.HasValue;

    // Informations
    public string GetStatut()
    {
        if (EstArchive) return "Archivée";
        if (EstValidee == true) return "Validée";
        if (EstValidee == false) return "Rejetée";
        return "En attente";
    }

    public string GetDescription()
    {
        var type = TypeTransport?.Libelle ?? "Transport";
        var heure = HeureTransport?.FormatHeure() ?? "--:--";
        var date = DateTransport?.FormatDate() ?? "--/--/----";
        
        return $"{type} - {date} à {heure}";
    }
}