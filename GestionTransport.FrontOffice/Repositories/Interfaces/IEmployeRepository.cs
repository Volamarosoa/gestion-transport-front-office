using GestionTransport.FrontOffice.Models.Employe;

namespace GestionTransport.FrontOffice.Repositories.Interfaces
{
    public interface IEmployeRepository : IRepository<EmployeModel>
    {
        List<EmployeModel> GetByDepartement(int idDepartement);
        List<EmployeModel> GetActifs();
        EmployeModel? GetByMatricule(string matricule);
        long CountByMatriculeStartingWith(string prefix);
    }
}
