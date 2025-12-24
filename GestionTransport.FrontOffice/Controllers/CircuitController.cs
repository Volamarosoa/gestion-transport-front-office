using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GestionTransport.FrontOffice.Models;

namespace GestionTransport.FrontOffice.Controllers;

public class CircuitController : Controller
{
    private readonly ILogger<CircuitController> _logger;

    public CircuitController(ILogger<CircuitController> logger)
    {
        _logger = logger;
    }

    public IActionResult MonCircuit()
    {
        // Afficher le circuit assigné à l'employé
        // - Nom du circuit
        // - Horaires
        // - Autres employés
        // - Carte du trajet
        return View();
    }    
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
