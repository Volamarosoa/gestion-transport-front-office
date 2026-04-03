using GestionTransport.FrontOffice.Models;
using GestionTransport.FrontOffice.Models.Affectation;
using GestionTransport.FrontOffice.Repositories.Interfaces;

namespace GestionTransport.FrontOffice.Services
{
    public class AffectationService
    {
        private readonly IAffectationRepository _affectationRepository;
        private readonly IEmployeRepository _employeRepository;
        private readonly IAdresseEmployeRepository _adresseRepository;
        private readonly ITypeTransportRepository _typeTransportRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IVehiculeRepository _vehiculeRepository;
        private readonly IHeureTransportRepository _heureTransportRepository;
        private readonly ITypeAffectationRepository _typeAffectationRepository;
        private readonly ILogger<AffectationService> _logger;

        public AffectationService(
            IAffectationRepository affectationRepository,
            IEmployeRepository employeRepository,
            IAdresseEmployeRepository adresseRepository,
            ITypeTransportRepository typeTransportRepository,
            ISiteRepository siteRepository,
            IVehiculeRepository vehiculeRepository,
            IHeureTransportRepository heureTransportRepository,
            ITypeAffectationRepository typeAffectationRepository,
            ILogger<AffectationService> logger)
        {
            _affectationRepository = affectationRepository;
            _employeRepository = employeRepository;
            _adresseRepository = adresseRepository;
            _typeTransportRepository = typeTransportRepository;
            _siteRepository = siteRepository;
            _vehiculeRepository = vehiculeRepository;
            _heureTransportRepository = heureTransportRepository;
            _typeAffectationRepository = typeAffectationRepository;
            _logger = logger;
        }

        // Récupérer les affectations d'un employé
        public List<AffectationModel> GetByEmploye(int idEmploye)
        {
            return _affectationRepository.GetByEmploye(idEmploye);
        }

        public AffectationModel GetById(int id)
        {
            var affectation = _affectationRepository.GetById(id);
            if (affectation == null)
                throw new KeyNotFoundException($"Affectation avec l'id {id} introuvable.");
            return affectation;
        }

        // Créer une demande de transport (avec auto-assignment)
        public int Create(DateTime date, int idEmploye, int idAdresse, int idTypeTransport,
            int idSite, int idHeureTransport, string? commentaire)
        {
            var now = DateTime.Now;
            var dateTransport = date.Date;

            if (dateTransport < now.Date)
                throw new InvalidOperationException("La date de transport ne peut pas etre inferieure a la date du jour.");

            var employe = _employeRepository.GetById(idEmploye);
            if (employe == null)
                throw new KeyNotFoundException($"Employe avec l'id {idEmploye} introuvable.");

            if (!employe.EstActif())
                throw new InvalidOperationException("Votre profil employe est inactif. Merci de contacter l'administrateur.");

            var adresse = _adresseRepository.GetById(idAdresse);
            if (adresse == null)
                throw new KeyNotFoundException($"Adresse avec l'id {idAdresse} introuvable.");

            var typeTransport = _typeTransportRepository.GetById(idTypeTransport);
            if (typeTransport == null)
                throw new KeyNotFoundException($"Type de transport avec l'id {idTypeTransport} introuvable.");

            var site = _siteRepository.GetById(idSite);
            if (site == null)
                throw new KeyNotFoundException($"Site avec l'id {idSite} introuvable.");

            var heureTransport = _heureTransportRepository.GetById(idHeureTransport);
            if (heureTransport == null)
                throw new KeyNotFoundException($"Heure de transport avec l'id {idHeureTransport} introuvable.");

            // Vérifier si une affectation existe déjà pour cette date/type
            var existing = _affectationRepository.GetByEmployeAndDate(idEmploye, date, idTypeTransport);
            if (existing != null)
                throw new InvalidOperationException(
                    "Vous avez déjà une demande de transport pour cette date et ce type de transport.");

            // Auto-assignment de véhicule
            int? idVehicule = TryAutoAssignVehicle(date, idHeureTransport, idSite, idTypeTransport);

            var isLateForTomorrow = IsLateTomorrowRequest(dateTransport, now);
            var shouldStayPending = !employe.EstBeneficiaire || isLateForTomorrow;
            var auditReason = BuildAuditReason(idEmploye, dateTransport, now, employe.EstBeneficiaire, isLateForTomorrow);
            var finalComment = BuildFinalComment(commentaire, auditReason);

            var typeAuto = _typeAffectationRepository.GetByLibelle("Automatique");
            var typeManuel = _typeAffectationRepository.GetByLibelle("Manuel");

            var affectation = new AffectationModel
            {
                DateTransport = dateTransport,
                IdEmploye = idEmploye,
                IdAdresse = idAdresse,
                IdTypeTransport = idTypeTransport,
                IdSite = idSite,
                IdVehicule = idVehicule,
                IdHeureTransport = idHeureTransport,
                EstValidee = shouldStayPending ? null : true,
                Commentaire = finalComment,
                DateCreation = now,
                DateValidation = shouldStayPending ? null : now,
                IdType = shouldStayPending
                    ? (typeManuel?.Id ?? typeAuto?.Id ?? 1)
                    : (typeAuto?.Id ?? typeManuel?.Id ?? 1),
                EstArchive = false
            };

            return _affectationRepository.Create(affectation);
        }

        // Annuler une demande en attente (employé annule sa propre demande)
        public void Annuler(int id, int idEmploye)
        {
            var affectation = _affectationRepository.GetById(id);
            if (affectation == null)
                throw new KeyNotFoundException($"Affectation avec l'id {id} introuvable.");

            if (affectation.IdEmploye != idEmploye)
                throw new InvalidOperationException("Vous ne pouvez annuler que vos propres demandes.");

            if (affectation.EstValidee.HasValue)
                throw new InvalidOperationException("Impossible d'annuler une demande déjà traitée.");

            if (affectation.EstArchive)
                throw new InvalidOperationException("Impossible d'annuler une demande archivée.");

            _affectationRepository.Delete(id);
        }

        private static bool IsLateTomorrowRequest(DateTime dateTransport, DateTime now)
        {
            if (dateTransport != now.Date.AddDays(1))
                return false;

            var cutoff = now.Date.AddHours(15);
            // Regle explicite: <= 15:00 accepte, > 15:00 en retard.
            return now > cutoff;
        }

        private string BuildAuditReason(int idEmploye, DateTime dateTransport, DateTime now, bool estBeneficiaire, bool isLateForTomorrow)
        {
            if (dateTransport != now.Date.AddDays(1))
            {
                var acceptedStatus = estBeneficiaire ? "validee automatiquement" : "en attente de validation";
                var reason = $"Decision technique: demande hors J-1; statut {acceptedStatus}.";
                _logger.LogInformation("Affectation {Decision}. EmployeId={EmployeId}, DateTransport={DateTransport:yyyy-MM-dd}", reason, idEmploye, dateTransport);
                return reason;
            }

            var cutoff = now.Date.AddHours(15);
            var timeStatus = now > cutoff ? "apres 15:00" : "avant ou a 15:00";
            string statusReason;

            if (!estBeneficiaire)
            {
                statusReason = $"Decision technique: En attente - employe non beneficiaire ({timeStatus}).";
            }
            else if (isLateForTomorrow)
            {
                statusReason = "Decision technique: En attente - demande J-1 apres 15:00.";
            }
            else
            {
                statusReason = "Decision technique: Validee automatiquement - demande J-1 avant ou a 15:00.";
            }

            _logger.LogInformation(
                "Regle cutoff appliquee. EmployeId={EmployeId}, DateTransport={DateTransport:yyyy-MM-dd}, Now={Now:HH:mm:ss}, Cutoff=15:00:00, IsLate={IsLate}, EstBeneficiaire={EstBeneficiaire}, Decision={Decision}",
                idEmploye,
                dateTransport,
                now,
                isLateForTomorrow,
                estBeneficiaire,
                statusReason);

            return statusReason;
        }

        private static string BuildFinalComment(string? userComment, string auditReason)
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(userComment))
                parts.Add(userComment.Trim());

            parts.Add(auditReason);

            var combined = string.Join(" | ", parts);
            return combined.Length <= 255 ? combined : combined.Substring(0, 255);
        }

        private int? TryAutoAssignVehicle(DateTime date, int idHeureTransport, int idSite, int idTypeTransport)
        {
            var vehiculesActifs = _vehiculeRepository.GetActifs()
                .OrderBy(v => v.NombrePlaces)
                .ToList();

            foreach (var vehicule in vehiculesActifs)
            {
                var count = _affectationRepository.CountByVehiculeAndDateAndHeure(
                    vehicule.Id, date, idHeureTransport);

                if (count < (vehicule.NombrePlaces ?? 0))
                    return vehicule.Id;
            }

            return null;
        }
    }
}
