using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GestionTransport.FrontOffice.Models;
using GestionTransport.FrontOffice.Repositories;

namespace GestionTransport.FrontOffice.Controllers;

public class CompteController : Controller
{
    private readonly ILogger<CompteController> _logger;
    private readonly ICompteRepository _compteRepo;

    public CompteController(ILogger<CompteController> logger, ICompteRepository compteRepo)
    {
        _logger = logger;
        _compteRepo = compteRepo;
    }

    public IActionResult MonProfil()
    {
        int employeId = 1;
        var employe = _compteRepo.GetById(employeId);
        
        if (employe == null)
        {
            return NotFound();
        }
            
        return View(employe);
    }

    public IActionResult MonAdresse()
    {
        // TODO: Récupérer l'ID de l'employé connecté
        int employeId = 1;
            
        var employe = _compteRepo.GetById(employeId);
            
        if (employe == null)
        {
            return NotFound();
        }
            
        return View(employe);
    }

    [HttpPost]
    public IActionResult ModifierAdresse(string adresseDepart, string adresseArrivee, double lat, double lng)
    {
        try
        {
            // TODO: Récupérer l'ID de l'employé connecté
            int employeId = 1;
                
            // Mettre à jour les adresses
            _compteRepo.UpdateAdresse(employeId, adresseDepart, adresseArrivee, lat, lng);
                
            _logger.LogInformation($"Adresses modifiées pour l'employé {employeId}");
                
            // TODO: Appeler API pour recalculer les routes de transport
                
            TempData["Success"] = "Adresses modifiées avec succès! Les routes seront recalculées.";
            return RedirectToAction("MonProfil");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la modification des adresses");
            TempData["Error"] = "Erreur lors de la modification des adresses";
            return RedirectToAction("MonProfil");
        }
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
