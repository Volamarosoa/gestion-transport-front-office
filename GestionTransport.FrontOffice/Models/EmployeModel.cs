namespace GestionTransport.FrontOffice.Models;

public class EmployeModel
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string AdresseDepart { get; set; }
    public string AdresseArrivee { get; set; }
    public string Telephone { get; set; }
}