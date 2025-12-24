using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GestionTransport.FrontOffice.Models;

namespace GestionTransport.FrontOffice.Controllers;

public class TransportController : Controller
{
    private readonly ILogger<TransportController> _logger;

    public TransportController(ILogger<TransportController> logger)
    {
        _logger = logger;
    }

    public IActionResult DemanderTransport()
    {
        return View();
    }

    [HttpPost]
    public IActionResult DemanderTransport(TransportModel model)
    {
        if (ModelState.IsValid)
        {
            // Enregistrer la demande
            // model.Date, model.Type (Aller/Retour), model.Raison, etc.
            
            TempData["Success"] = "Votre demande de transport a été soumise avec succès!";
            return RedirectToAction("MesTransports");
        }
        return View(model);
    }

    public IActionResult MesTransports()
    {
        // Récupérer toutes les demandes de l'employé connecté
        // Afficher: Date, Type, Statut (En attente, Approuvé, Refusé)
        return View();
    }

    // GET: Détails d'une demande
    public IActionResult DetailsTransport(int id)
    {
        return View();
    }

    // POST: Annuler une demande
    [HttpPost]
    public IActionResult AnnulerTransport(int id)
    {
        // Annuler la demande si elle n'est pas encore traitée
        TempData["Success"] = "Demande annulée avec succès!";
        return RedirectToAction("MesTransports");
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
