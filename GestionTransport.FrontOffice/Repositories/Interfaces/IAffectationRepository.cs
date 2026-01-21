using GestionTransport.FrontOffice.Models.Affectation;

namespace GestionTransport.FrontOffice.Repositories.Interfaces
{
    public interface IAffectationRepository : IRepository<AffectationModel>
    {
        List<AffectationModel> GetByDate(int idDate);
        List<AffectationModel> GetByEmploye(int idEmploye);
        List<AffectationModel> GetByVehicule(int idVehicule);
        List<AffectationModel> GetEnAttente();
        List<AffectationModel> GetValidees();
        List<AffectationModel> GetRejetees();
        List<AffectationModel> GetNonArchivees();
        List<AffectationModel> GetArchivees();
        void Valider(int id, string? commentaire = null);
        void Rejeter(int id, string? commentaire = null);
        void Archiver(int id);
        void ArchiverParDate(int idDate);
        void ArchiverDatesPassees();
        AffectationModel GetByEmployeAndDate(int idEmploye, int idDate, int idTypeTransport);
        List<AffectationModel> GetByDateAndTypeTransport(int idDate, int idTypeTransport);
    }
}