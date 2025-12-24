using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GestionTransport.FrontOffice.Models;

namespace GestionTransport.FrontOffice.Controllers;

public class CompteController : Controller
{
    private readonly ILogger<CompteController> _logger;

    public CompteController(ILogger<CompteController> logger)
    {
        _logger = logger;
    }

    public IActionResult MonProfil()
    {
        return View();
    }

    public IActionResult MonAdresse()
    {
        // IMPORTANT: Permet à l'employé de mettre à jour son adresse
        // Quand modifiée → déclenche recalcul des routes
        return View();
    }

    [HttpPost]
    public IActionResult ModifierAdresse(string newAddress, double lat, double lng)
    {
        // Mettre à jour l'adresse
        // Appeler API pour recalculer les routes
        return RedirectToAction("MonAdresse");
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
