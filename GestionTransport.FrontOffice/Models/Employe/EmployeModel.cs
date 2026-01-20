namespace GestionTransport.FrontOffice.Models.Employe;

public class EmployeModel
{
    public int Id { get; set; }

    public string Nom { get; set; } = string.Empty;

    public string Prenom { get; set; } = string.Empty;

    public string? Matricule { get; set; }

    public string? Telephone { get; set; }

    public int IdDepartement { get; set; }

    public bool Actif { get; set; } = true;

    public DateTime DateInsertion { get; set; } = DateTime.Now;

    public DateTime? DateDesactivation { get; set; }

    public DepartementModel? Departement { get; set; }
    
    public List<AdresseEmployeModel>? Adresses { get; set; }

    public string NomComplet() => $"{Prenom} {Nom}";

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
    
}