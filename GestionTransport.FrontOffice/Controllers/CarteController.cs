using Microsoft.AspNetCore.Mvc;
using GestionTransport.FrontOffice.Models;
using GestionTransport.FrontOffice.Models.Employe;
using GestionTransport.FrontOffice.Repositories.Interfaces;

namespace GestionTransport.FrontOffice.Controllers
{
    public class CarteController : Controller
    {
        // Simulated logged-in employee ID (replace with session later)
        private const int CURRENT_EMPLOYE_ID = 1;

        private readonly IEmployeRepository _employeRepository;
        private readonly IAdresseEmployeRepository _adresseEmployeRepository;
        private readonly ISiteRepository _siteRepository;

        public CarteController(
            IEmployeRepository employeRepository,
            IAdresseEmployeRepository adresseEmployeRepository,
            ISiteRepository siteRepository)
        {
            _employeRepository = employeRepository;
            _adresseEmployeRepository = adresseEmployeRepository;
            _siteRepository = siteRepository;
        }

        // GET: Display map with all addresses and sites
        public IActionResult Index()
        {
            var employe = _employeRepository.GetById(CURRENT_EMPLOYE_ID);
            var mesAdresses = _adresseEmployeRepository.GetActifsByEmploye(CURRENT_EMPLOYE_ID);
            var sites = _siteRepository.GetAvecCoordonnees().Where(s => s.EstActif()).ToList();

            ViewBag.Employe = employe;
            ViewBag.MesAdresses = mesAdresses;
            ViewBag.Sites = sites;

            return View();
        }

        // POST: Add new address for employee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AjouterAdresse(string adresse, decimal latitude, decimal longitude, bool estPrincipale = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(adresse))
                {
                    return Json(new { success = false, message = "L'adresse est obligatoire." });
                }

                if (latitude == 0 && longitude == 0)
                {
                    return Json(new { success = false, message = "Veuillez sélectionner un point sur la carte." });
                }

                // Persistance
                var nouvelleAdresse = new AdresseEmployeModel
                {
                    IdEmploye = CURRENT_EMPLOYE_ID,
                    Adresse = adresse,
                    Latitude = latitude,
                    Longitude = longitude,
                    EstPrincipale = estPrincipale,
                    Actif = true,
                    DateInsertion = DateTime.Now
                };

                var newId = _adresseEmployeRepository.Create(nouvelleAdresse);

                // Assurer l'unicité de l'adresse principale
                if (estPrincipale)
                {
                    _adresseEmployeRepository.SetAsMain(newId, CURRENT_EMPLOYE_ID);
                }

                nouvelleAdresse.Id = newId;

                return Json(new 
                { 
                    success = true, 
                    message = "Adresse ajoutée avec succès!",
                    adresse = nouvelleAdresse
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        // POST: Delete address
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SupprimerAdresse(int id)
        {
            try
            {
                var adresse = _adresseEmployeRepository.GetById(id);

                if (adresse == null || adresse.IdEmploye != CURRENT_EMPLOYE_ID)
                {
                    return Json(new { success = false, message = "Adresse non trouvée." });
                }

                _adresseEmployeRepository.Delete(id);

                return Json(new { success = true, message = "Adresse supprimée avec succès!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }
    }
}