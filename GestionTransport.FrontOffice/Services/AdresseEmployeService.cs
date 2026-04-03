using GestionTransport.FrontOffice.Models.Employe;
using GestionTransport.FrontOffice.Repositories.Interfaces;

namespace GestionTransport.FrontOffice.Services
{
    public class AdresseEmployeService
    {
        private readonly IAdresseEmployeRepository _adresseRepository;
        private readonly IEmployeRepository _employeRepository;

        public AdresseEmployeService(IAdresseEmployeRepository adresseRepository, IEmployeRepository employeRepository)
        {
            _adresseRepository = adresseRepository;
            _employeRepository = employeRepository;
        }

        public List<AdresseEmployeModel> GetByEmploye(int idEmploye)
        {
            return _adresseRepository.GetActifsByEmploye(idEmploye);
        }

        public AdresseEmployeModel GetById(int id)
        {
            var adresse = _adresseRepository.GetById(id);
            if (adresse == null)
                throw new KeyNotFoundException($"Adresse avec l'id {id} introuvable.");
            return adresse;
        }

        public int Create(int idEmploye, string adresse, decimal latitude, decimal longitude, bool estPrincipale)
        {
            var employe = _employeRepository.GetById(idEmploye);
            if (employe == null)
                throw new KeyNotFoundException($"Employé avec l'id {idEmploye} introuvable.");

            var model = new AdresseEmployeModel
            {
                IdEmploye = idEmploye,
                Adresse = adresse,
                Latitude = latitude,
                Longitude = longitude,
                EstPrincipale = estPrincipale,
                Actif = true,
                DateInsertion = DateTime.Now
            };

            return _adresseRepository.Create(model);
        }

        public void Update(int id, string adresse, decimal latitude, decimal longitude, bool estPrincipale)
        {
            var existing = _adresseRepository.GetById(id);
            if (existing == null)
                throw new KeyNotFoundException($"Adresse avec l'id {id} introuvable.");

            existing.Adresse = adresse;
            existing.Latitude = latitude;
            existing.Longitude = longitude;
            existing.EstPrincipale = estPrincipale;
            _adresseRepository.Update(existing);
        }

        public void Deactivate(int id)
        {
            var adresse = _adresseRepository.GetById(id);
            if (adresse == null)
                throw new KeyNotFoundException($"Adresse avec l'id {id} introuvable.");

            _adresseRepository.Deactivate(id);
        }

        public void Activate(int id)
        {
            _adresseRepository.Activate(id);
        }

        public void SetAsMain(int id, int idEmploye)
        {
            _adresseRepository.SetAsMain(id, idEmploye);
        }
    }
}
