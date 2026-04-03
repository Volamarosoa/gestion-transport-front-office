using GestionTransport.FrontOffice.Models.Affectation;

namespace GestionTransport.FrontOffice.Repositories.Interfaces
{
    public interface IAffectationRepository : IRepository<AffectationModel>
    {
        List<AffectationModel> GetByDate(DateTime date);
        List<AffectationModel> GetByEmploye(int idEmploye);
        List<AffectationModel> GetByVehicule(int idVehicule);
        List<AffectationModel> GetEnAttente();
        List<AffectationModel> GetValidees();
        List<AffectationModel> GetValidatedAffectationsToday();
        List<AffectationModel> GetRejetees();
        List<AffectationModel> GetNonArchivees();
        List<AffectationModel> GetArchivees();
        void Valider(int id, string? commentaire = null);
        void Rejeter(int id, string? commentaire = null);
        void Archiver(int id);
        void ArchiverDatesPassees();
        AffectationModel GetByEmployeAndDate(int idEmploye, DateTime date, int idTypeTransport);
        List<AffectationModel> GetByDateAndTypeTransport(DateTime date, int idTypeTransport);
        List<AffectationModel> GetByDateAndHeureAndSiteAndType(DateTime date, int idHeureTransport, int idSite, int idTypeTransport);
        int CountByVehiculeAndDateAndHeure(int idVehicule, DateTime date, int idHeureTransport);
        void UpdateVehicule(int id, int? idVehicule);
    }
}
