using GestionTransport.FrontOffice.Models.Affectation;

namespace GestionTransport.FrontOffice.Repositories.Interfaces
{
    public interface ITypeAffectationRepository : IRepository<TypeAffectationModel>
    {
        List<TypeAffectationModel> GetActifs();
        TypeAffectationModel GetByLibelle(string libelle);
    }
}