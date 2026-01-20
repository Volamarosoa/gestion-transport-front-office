using GestionTransport.FrontOffice.Models;

namespace GestionTransport.FrontOffice.Repositories.Interfaces
{
    public interface ITypeTransportRepository : IRepository<TypeTransportModel>
    {
        List<TypeTransportModel> GetActifs();
        TypeTransportModel GetByLibelle(string libelle);
    }
}