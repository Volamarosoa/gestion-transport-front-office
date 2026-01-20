using GestionTransport.FrontOffice.Models;

namespace GestionTransport.FrontOffice.Repositories.Interfaces
{
    public interface ISiteRepository : IRepository<SiteModel>
    {
        List<SiteModel> GetActifs();
        List<SiteModel> GetAvecCoordonnees();
        SiteModel GetByNom(string nom);
    }
}