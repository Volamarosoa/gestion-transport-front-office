using GestionTransport.FrontOffice.Models;

namespace GestionTransport.FrontOffice.Repositories.Interfaces
{
    public interface IDepartementRepository : IRepository<DepartementModel>
    {
        List<DepartementModel> GetActifs();
        DepartementModel GetByNom(string nom);
    }
}