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

            var typeLibelle = typeTransport.Libelle?.Trim() ?? string.Empty;

            if (dateTransport == now.Date && IsAller(typeLibelle))
                throw new InvalidOperationException("Un transport Aller pour aujourd'hui doit etre demande hier avant 15:00.");
            
            if (dateTransport == now.Date.AddDays(1) && IsAller(typeLibelle) && now > now.Date.AddHours(15))
                throw new InvalidOperationException(
                    "Apres 15:00, vous ne pouvez plus demander l'Aller pour demain. " +
                    "Aller-Retour est aussi refuse. Vous pouvez seulement demander un Retour.");

            if (dateTransport == now.Date && IsRetour(typeLibelle) && now > now.Date.AddHours(15))
                throw new InvalidOperationException("Un transport Retour pour aujourd'hui doit etre demande avant ou a 15:00.");

            var site = _siteRepository.GetById(idSite);
            if (site == null)
                throw new KeyNotFoundException($"Site avec l'id {idSite} introuvable.");

            var heureTransport = _heureTransportRepository.GetById(idHeureTransport);
            if (heureTransport == null)
                throw new KeyNotFoundException($"Heure de transport avec l'id {idHeureTransport} introuvable.");

            // Retour seul pour aujourd'hui: l'heure souhaitee doit etre >= a l'heure courante.
            if (dateTransport == now.Date
                && IsRetour(typeLibelle)
                && !IsAller(typeLibelle)
                && heureTransport.Heure.HasValue
                && heureTransport.Heure.Value < now.TimeOfDay)
            {
                throw new InvalidOperationException(
                    "Pour un Retour aujourd'hui, l'heure souhaitee ne peut pas etre inferieure a l'heure actuelle.");
            }


            // Vérifier si une affectation existe déjà pour cette date/type
            var existing = _affectationRepository.GetByEmployeAndDate(idEmploye, date, idTypeTransport);
            if (existing != null)
                throw new InvalidOperationException(
                    "Vous avez déjà une demande de transport pour cette date et ce type de transport.");

            // Auto-assignment de véhicule
            int? idVehicule = TryAutoAssignVehicle(date, idHeureTransport, idSite, idTypeTransport);

            var cutoff = ResolveCutoff(dateTransport, typeLibelle);
            // Regle explicite: <= 15:00 accepte, > 15:00 en retard.
            var isLate = now > cutoff;
            var shouldStayPending = !employe.EstBeneficiaire || isLate;
            var auditReason = BuildAuditReason(idEmploye, dateTransport, typeLibelle, now, cutoff, employe.EstBeneficiaire, isLate, shouldStayPending);
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

        private static DateTime ResolveCutoff(DateTime dateTransport, string typeLibelle)
        {
            // Aller (et Aller-Retour) : J-1 15:00, Retour : jour J 15:00.
            if (IsRetour(typeLibelle) && !IsAller(typeLibelle))
                return dateTransport.Date.AddHours(15);

            return dateTransport.Date.AddDays(-1).AddHours(15);
        }

        private static bool IsAller(string typeLibelle)
        {
            return typeLibelle.Contains("aller", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsRetour(string typeLibelle)
        {
            return typeLibelle.Contains("retour", StringComparison.OrdinalIgnoreCase);
        }

        private string BuildAuditReason(int idEmploye, DateTime dateTransport, string typeLibelle, DateTime now, DateTime cutoff, bool estBeneficiaire, bool isLate, bool isPending)
        {
            var decision = isPending ? "En attente" : "Validee automatiquement";
            var ruleContext = isLate
                ? $"Demande en retard ({now:yyyy-MM-dd HH:mm:ss} > cutoff {cutoff:yyyy-MM-dd HH:mm:ss})"
                : $"Demande dans le delai ({now:yyyy-MM-dd HH:mm:ss} <= cutoff {cutoff:yyyy-MM-dd HH:mm:ss})";

            var reason = $"Decision technique: {decision} - Type={typeLibelle}; {ruleContext}; EstBeneficiaire={estBeneficiaire}.";

            _logger.LogInformation(
                "Regle transport appliquee. EmployeId={EmployeId}, Type={Type}, DateTransport={DateTransport:yyyy-MM-dd}, Now={Now:yyyy-MM-dd HH:mm:ss}, Cutoff={Cutoff:yyyy-MM-dd HH:mm:ss}, IsLate={IsLate}, EstBeneficiaire={EstBeneficiaire}, Decision={Decision}",
                idEmploye,
                typeLibelle,
                dateTransport,
                now,
                cutoff,
                isLate,
                estBeneficiaire,
                decision);

            return reason;
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
