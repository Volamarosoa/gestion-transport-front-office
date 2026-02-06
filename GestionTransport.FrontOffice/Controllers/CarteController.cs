using Microsoft.AspNetCore.Mvc;
using GestionTransport.FrontOffice.Models;
using GestionTransport.FrontOffice.Models.Employe;

namespace GestionTransport.FrontOffice.Controllers
{
    public class CarteController : Controller
    {
        // Simulated logged-in employee ID (replace with session later)
        private const int CURRENT_EMPLOYE_ID = 1;

        // Static data for sites
        private static List<SiteModel> GetSites()
        {
            return new List<SiteModel>
            {
                new SiteModel
                {
                    Id = 1,
                    Nom = "Siège Social",
                    Adresse = "Ankorondrano, Antananarivo",
                    Latitude = -18.9010m,
                    Longitude = 47.5270m,
                    Actif = true
                },
                new SiteModel
                {
                    Id = 2,
                    Nom = "Usine Ankadimbahoaka",
                    Adresse = "Ankadimbahoaka, Antananarivo",
                    Latitude = -18.8530m,
                    Longitude = 47.5380m,
                    Actif = true
                },
                new SiteModel
                {
                    Id = 3,
                    Nom = "Agence Antsirabe",
                    Adresse = "Antsirabe, Madagascar",
                    Latitude = -19.8658m,
                    Longitude = 47.0368m,
                    Actif = true
                }
            };
        }

        // Static data for employees
        private static List<EmployeModel> GetEmployes()
        {
            return new List<EmployeModel>
            {
                new EmployeModel
                {
                    Id = 1,
                    Nom = "Rakoto",
                    Prenom = "Jean",
                    Matricule = "EMP001",
                    Telephone = "0341234567"
                }
            };
        }

        // Static data for employee addresses
        private static List<AdresseEmployeModel> _adressesEmployes = new List<AdresseEmployeModel>
        {
            new AdresseEmployeModel
            {
                Id = 1,
                IdEmploye = 1,
                Adresse = "Analakely, Antananarivo",
                Latitude = -18.9145m,
                Longitude = 47.5265m,
                EstPrincipale = true,
                Actif = true,
                DateInsertion = DateTime.Now.AddDays(-30)
            },
            new AdresseEmployeModel
            {
                Id = 2,
                IdEmploye = 1,
                Adresse = "Behoririka, Antananarivo",
                Latitude = -18.9088m,
                Longitude = 47.5239m,
                EstPrincipale = false,
                Actif = true,
                DateInsertion = DateTime.Now.AddDays(-15)
            }
        };

        // GET: Display map with all addresses and sites
        public IActionResult Index()
        {
            var employe = GetEmployes().FirstOrDefault(e => e.Id == CURRENT_EMPLOYE_ID);
            var mesAdresses = _adressesEmployes.Where(a => a.IdEmploye == CURRENT_EMPLOYE_ID && a.Actif).ToList();
            var sites = GetSites().Where(s => s.EstActif()).ToList();

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

                // If setting as principal, unset other principal addresses
                if (estPrincipale)
                {
                    foreach (var addr in _adressesEmployes.Where(a => a.IdEmploye == CURRENT_EMPLOYE_ID && a.EstPrincipale))
                    {
                        addr.EstPrincipale = false;
                    }
                }

                var nouvelleAdresse = new AdresseEmployeModel
                {
                    Id = _adressesEmployes.Any() ? _adressesEmployes.Max(a => a.Id) + 1 : 1,
                    IdEmploye = CURRENT_EMPLOYE_ID,
                    Adresse = adresse,
                    Latitude = latitude,
                    Longitude = longitude,
                    EstPrincipale = estPrincipale,
                    Actif = true,
                    DateInsertion = DateTime.Now
                };

                _adressesEmployes.Add(nouvelleAdresse);

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
                var adresse = _adressesEmployes.FirstOrDefault(a => a.Id == id && a.IdEmploye == CURRENT_EMPLOYE_ID);
                
                if (adresse == null)
                {
                    return Json(new { success = false, message = "Adresse non trouvée." });
                }

                adresse.Actif = false;
                adresse.DateDesactivation = DateTime.Now;

                return Json(new { success = true, message = "Adresse supprimée avec succès!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }
    }
}