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

        public AffectationService(
            IAffectationRepository affectationRepository,
            IEmployeRepository employeRepository,
            IAdresseEmployeRepository adresseRepository,
            ITypeTransportRepository typeTransportRepository,
            ISiteRepository siteRepository,
            IVehiculeRepository vehiculeRepository,
            IHeureTransportRepository heureTransportRepository,
            ITypeAffectationRepository typeAffectationRepository)
        {
            _affectationRepository = affectationRepository;
            _employeRepository = employeRepository;
            _adresseRepository = adresseRepository;
            _typeTransportRepository = typeTransportRepository;
            _siteRepository = siteRepository;
            _vehiculeRepository = vehiculeRepository;
            _heureTransportRepository = heureTransportRepository;
            _typeAffectationRepository = typeAffectationRepository;
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
            var employe = _employeRepository.GetById(idEmploye);
            if (employe == null)
                throw new KeyNotFoundException($"Employé avec l'id {idEmploye} introuvable.");

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
            var typeAuto = _typeAffectationRepository.GetByLibelle("Automatique");
            int idType = typeAuto?.Id ?? 1;

            var affectation = new AffectationModel
            {
                DateTransport = date.Date,
                IdEmploye = idEmploye,
                IdAdresse = idAdresse,
                IdTypeTransport = idTypeTransport,
                IdSite = idSite,
                IdVehicule = idVehicule,
                IdHeureTransport = idHeureTransport,
                EstValidee = null,
                Commentaire = commentaire,
                DateCreation = DateTime.Now,
                IdType = idType,
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
