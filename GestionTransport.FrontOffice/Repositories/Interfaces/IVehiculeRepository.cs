using GestionTransport.FrontOffice.Models;

namespace GestionTransport.FrontOffice.Repositories.Interfaces
{
    public interface IVehiculeRepository : IRepository<VehiculeModel>
    {
        List<VehiculeModel> GetActifs();
        List<VehiculeModel> GetDisponibles(int placesRequises);
        VehiculeModel GetByMatricule(string matricule);
    }
}