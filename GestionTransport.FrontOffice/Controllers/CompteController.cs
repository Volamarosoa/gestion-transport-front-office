using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GestionTransport.FrontOffice.Models;
using GestionTransport.FrontOffice.Models.Employe;

using GestionTransport.FrontOffice.Services;
using GestionTransport.FrontOffice.Repositories.Interfaces;

namespace GestionTransport.FrontOffice.Controllers;

public class CompteController : Controller
{
    private readonly ILogger<CompteController> _logger;
    private readonly IEmployeRepository _employeRepository;
    private readonly IAdresseEmployeRepository _adresseEmployeRepository;
    private const int CURRENT_EMPLOYE_ID = 1; // TODO: remplacer par l'id de l'utilisateur connecté

    public CompteController(
        ILogger<CompteController> logger,
        IEmployeRepository employeRepository,
        IAdresseEmployeRepository adresseEmployeRepository)
    {
        _logger = logger;
        _employeRepository = employeRepository;
        _adresseEmployeRepository = adresseEmployeRepository;
    }

    // Profil du client / adresses
    public IActionResult MonProfil()
    {
        var employe = _employeRepository.GetById(CURRENT_EMPLOYE_ID);
        var adresses = _adresseEmployeRepository.GetByEmploye(CURRENT_EMPLOYE_ID);

        ViewBag.AdressePrincipale = adresses.FirstOrDefault(a => a.EstPrincipale);
        ViewBag.AutreAdresse = adresses.FirstOrDefault(a => !a.EstPrincipale);

        return View("../Compte/MonProfil", employe);
    }

    public IActionResult MonAdresse()
    {
        var adresses = _adresseEmployeRepository.GetByEmploye(CURRENT_EMPLOYE_ID);
        return View("MesAdresses", adresses);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ModifierAdresse(string adresseDepart, string adresseArrivee, decimal? lat, decimal? lng)
    {
        try
        {
            var adresses = _adresseEmployeRepository.GetByEmploye(CURRENT_EMPLOYE_ID);

            // Adresse principale (départ)
            var principale = adresses.FirstOrDefault(a => a.EstPrincipale) ?? new AdresseEmployeModel { IdEmploye = CURRENT_EMPLOYE_ID, EstPrincipale = true, Actif = true, DateInsertion = DateTime.Now };
            principale.Adresse = adresseDepart;
            principale.Latitude = lat;
            principale.Longitude = lng;
            principale.EstPrincipale = true;

            if (principale.Id == 0)
            {
                var id = _adresseEmployeRepository.Create(principale);
                _adresseEmployeRepository.SetAsMain(id, CURRENT_EMPLOYE_ID);
            }
            else
            {
                _adresseEmployeRepository.Update(principale);
                _adresseEmployeRepository.SetAsMain(principale.Id, CURRENT_EMPLOYE_ID);
            }

            // Adresse de travail / arrivée (non principale)
            if (!string.IsNullOrWhiteSpace(adresseArrivee))
            {
                var secondaire = adresses.FirstOrDefault(a => !a.EstPrincipale) ?? new AdresseEmployeModel { IdEmploye = CURRENT_EMPLOYE_ID, EstPrincipale = false, Actif = true, DateInsertion = DateTime.Now };
                secondaire.Adresse = adresseArrivee;
                secondaire.Latitude = lat;
                secondaire.Longitude = lng;
                secondaire.EstPrincipale = false;

                if (secondaire.Id == 0)
                {
                    _adresseEmployeRepository.Create(secondaire);
                }
                else
                {
                    _adresseEmployeRepository.Update(secondaire);
                }
            }

            TempData["Success"] = "Adresses mises à jour.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Erreur lors de la mise à jour : {ex.Message}";
        }

        return RedirectToAction("MonProfil");
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}