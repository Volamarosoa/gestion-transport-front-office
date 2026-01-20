namespace GestionTransport.FrontOffice.Models.Affectation;

public class TypeAffectationModel
{
    public int Id { get; set; }

    public string? Libelle { get; set; } // Automatique, Manuel

    public bool Actif { get; set; } = true;

    public bool EstAutomatique() => Libelle?.ToLower().Contains("automatique") ?? false;

    public bool EstManuel() => Libelle?.ToLower().Contains("manuel") ?? false;
}