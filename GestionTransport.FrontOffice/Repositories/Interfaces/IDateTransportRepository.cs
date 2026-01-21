using GestionTransport.FrontOffice.Models.Transport;

namespace GestionTransport.FrontOffice.Repositories.Interfaces
{
    public interface IDateTransportRepository : IRepository<DateTransportModel>
    {
        List<DateTransportModel> GetActifs();
        List<DateTransportModel> GetFutures();
        List<DateTransportModel> GetByPeriode(DateTime dateDebut, DateTime dateFin);
        DateTransportModel GetByDate(DateTime date);
        DateTransportModel GetOrCreateByDate(DateTime date);
    }
}