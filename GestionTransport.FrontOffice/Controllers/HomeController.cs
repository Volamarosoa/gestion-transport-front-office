using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GestionTransport.FrontOffice.Models;

namespace GestionTransport.FrontOffice.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    // Réserver un transport
    public IActionResult ReserverTransport()
    {
        return View();
    }

    // Mes réservations (liste des transports du client)
    public IActionResult MesReservations()
    {
        return View();
    }

    // Suivi de transport en temps réel
    public IActionResult SuiviTransport()
    {
        return View();
    }

    // Historique des transports
    public IActionResult Historique()
    {
        return View();
    }

    // Profil du client
    public IActionResult MonProfil()
    {
        return View();
    }

    // Facturation et paiements
    public IActionResult Facturation()
    {
        return View();
    }

    // Support client
    public IActionResult Support()
    {
        return View();
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
