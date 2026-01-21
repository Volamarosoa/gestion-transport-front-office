using GestionTransport.FrontOffice.Models.Employe;

namespace GestionTransport.FrontOffice.Repositories.Interfaces
{
    public interface IAdresseEmployeRepository : IRepository<AdresseEmployeModel>
    {
        List<AdresseEmployeModel> GetByEmploye(int idEmploye);
        List<AdresseEmployeModel> GetActifsByEmploye(int idEmploye);
        AdresseEmployeModel GetAdressePrincipale(int idEmploye);
        void SetAsMain(int id, int idEmploye);
        List<AdresseEmployeModel> GetAvecCoordonnees();
    }
}