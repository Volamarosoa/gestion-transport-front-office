using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GestionTransport.FrontOffice.Models;
using GestionTransport.FrontOffice.Models.Employe;
using GestionTransport.FrontOffice.Extensions;

using GestionTransport.FrontOffice.Services;
using GestionTransport.FrontOffice.Repositories.Interfaces;

namespace GestionTransport.FrontOffice.Controllers;

[Authorize]
public class CompteController : Controller
{
    private readonly ILogger<CompteController> _logger;
    private readonly IEmployeRepository _employeRepository;
    private readonly IAdresseEmployeRepository _adresseEmployeRepository;

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
        var employeId = User.GetEmployeId();
        var employe = _employeRepository.GetById(employeId);
        var adresses = _adresseEmployeRepository.GetByEmploye(employeId);

        ViewBag.AdressePrincipale = adresses.FirstOrDefault(a => a.EstPrincipale);
        ViewBag.AutreAdresse = adresses.FirstOrDefault(a => !a.EstPrincipale);

        return View("../Compte/MonProfil", employe);
    }

    public IActionResult MonAdresse()
    {
        var adresses = _adresseEmployeRepository.GetByEmploye(User.GetEmployeId());
        return View("MesAdresses", adresses);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DefinirAdresseActuelle(int id)
    {
        try
        {
            var employeId = User.GetEmployeId();
            var adresse = _adresseEmployeRepository.GetById(id);

            if (adresse == null || adresse.IdEmploye != employeId)
            {
                TempData["Error"] = "Adresse introuvable.";
                return RedirectToAction("MonAdresse");
            }

            _adresseEmployeRepository.SetAsMain(id, employeId);
            TempData["Success"] = "Adresse actuelle mise a jour.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Erreur lors de la mise a jour: {ex.Message}";
        }

        return RedirectToAction("MonAdresse");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ImportAdressesCsv(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Veuillez selectionner un fichier CSV.";
            return RedirectToAction("MonAdresse");
        }

        if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            TempData["Error"] = "Le fichier doit etre au format CSV.";
            return RedirectToAction("MonAdresse");
        }

        try
        {
            var employeId = User.GetEmployeId();
            var imported = 0;
            var skipped = 0;

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);

            var headerLine = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(headerLine))
            {
                TempData["Error"] = "Le fichier CSV est vide.";
                return RedirectToAction("MonAdresse");
            }

            var delimiter = headerLine.Contains(';') ? ';' : ',';

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var parts = line.Split(delimiter);
                if (parts.Length == 0 || string.IsNullOrWhiteSpace(parts[0]))
                {
                    skipped++;
                    continue;
                }

                var adresseText = parts[0].Trim();
                decimal? latitude = null;
                decimal? longitude = null;
                var estPrincipale = false;

                if (parts.Length > 1 && decimal.TryParse(parts[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var lat))
                {
                    latitude = lat;
                }

                if (parts.Length > 2 && decimal.TryParse(parts[2].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var lng))
                {
                    longitude = lng;
                }

                if (parts.Length > 3)
                {
                    var flag = parts[3].Trim().ToLowerInvariant();
                    estPrincipale = flag == "1" || flag == "true" || flag == "oui" || flag == "yes";
                }

                var newId = _adresseEmployeRepository.Create(new AdresseEmployeModel
                {
                    IdEmploye = employeId,
                    Adresse = adresseText,
                    Latitude = latitude,
                    Longitude = longitude,
                    EstPrincipale = estPrincipale,
                    Actif = true,
                    DateInsertion = DateTime.Now
                });

                if (estPrincipale)
                {
                    _adresseEmployeRepository.SetAsMain(newId, employeId);
                }

                imported++;
            }

            TempData["Success"] = $"Import termine: {imported} adresse(s) ajoutee(s), {skipped} ligne(s) ignoree(s).";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Erreur lors de l'import CSV: {ex.Message}";
        }

        return RedirectToAction("MonAdresse");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ModifierAdresse(string adresseDepart, string adresseArrivee, decimal? lat, decimal? lng)
    {
        try
        {
            var employeId = User.GetEmployeId();
            var adresses = _adresseEmployeRepository.GetByEmploye(employeId);

            // Adresse principale (départ)
            var principale = adresses.FirstOrDefault(a => a.EstPrincipale) ?? new AdresseEmployeModel { IdEmploye = employeId, EstPrincipale = true, Actif = true, DateInsertion = DateTime.Now };
            principale.Adresse = adresseDepart;
            principale.Latitude = lat;
            principale.Longitude = lng;
            principale.EstPrincipale = true;

            if (principale.Id == 0)
            {
                var id = _adresseEmployeRepository.Create(principale);
                _adresseEmployeRepository.SetAsMain(id, employeId);
            }
            else
            {
                _adresseEmployeRepository.Update(principale);
                _adresseEmployeRepository.SetAsMain(principale.Id, employeId);
            }

            // Adresse de travail / arrivée (non principale)
            if (!string.IsNullOrWhiteSpace(adresseArrivee))
            {
                var secondaire = adresses.FirstOrDefault(a => !a.EstPrincipale) ?? new AdresseEmployeModel { IdEmploye = employeId, EstPrincipale = false, Actif = true, DateInsertion = DateTime.Now };
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