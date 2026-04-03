using GestionTransport.FrontOffice.Models.Affectation;

namespace GestionTransport.FrontOffice.Repositories.Interfaces
{
    public interface IHistoriqueAffectationRepository
    {
        List<HistoriqueAffectationModel> GetByAffectation(int idAffectation);
        int Create(HistoriqueAffectationModel historique);
    }
}
