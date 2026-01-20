using GestionTransport.FrontOffice.Models;

namespace GestionTransport.FrontOffice.Repositories.Interfaces
{
    public interface IHeureTransportRepository : IRepository<HeureTransportModel>
    {
        List<HeureTransportModel> GetActifs();
        List<HeureTransportModel> GetByPeriode(string libelle);
        HeureTransportModel GetByHeure(TimeSpan heure);
    }
}