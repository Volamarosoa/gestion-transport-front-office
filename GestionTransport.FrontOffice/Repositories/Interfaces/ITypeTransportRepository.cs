using GestionTransport.FrontOffice.Models.Transport;

namespace GestionTransport.FrontOffice.Repositories.Interfaces
{
    public interface ITypeTransportRepository : IRepository<TypeTransportModel>
    {
        List<TypeTransportModel> GetActifs();
        TypeTransportModel GetByLibelle(string libelle);
    }
}