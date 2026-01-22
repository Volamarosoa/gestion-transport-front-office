using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GestionTransport.FrontOffice.Models;
using GestionTransport.FrontOffice.Models.Affectation;
using GestionTransport.FrontOffice.Models.Employe;
using GestionTransport.FrontOffice.Models.Transport;
using GestionTransport.FrontOffice.Models.Utils;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionTransport.FrontOffice.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // Données statiques pour tester (avant de connecter la BDD)
    private static List<AffectationModel> GetStaticAffectations()
    {
        var affectations = new List<AffectationModel>();
        
        var employes = new[] { "Jean Dupont", "Marie Martin", "Pierre Bernard", "Sophie Lefebvre", 
                              "Luc Moreau", "Julie Petit", "François Roux", "Isabelle Blanc",
                              "Thomas Fournier", "Nathalie Girard", "Michel Bonnet", "Christine Lambert" };
        
        var sites = new[] { "Siège Paris", "Usine Lyon", "Agence Marseille", "Entrepôt Toulouse", 
                           "Siège Lille", "Agence Bordeaux", "Usine Nantes" };
        
        var adresses = new[] { "15 Rue de la Paix, Paris", "8 Avenue Foch, Lyon", 
                              "22 Boulevard Longchamp, Marseille", "45 Rue Alsace Lorraine, Toulouse" };

        // Générer 50 affectations pour tester la pagination
        for (int i = 1; i <= 50; i++)
        {
            var dateJour = DateTime.Now.AddDays(-i);
            var estValidee = i % 4 == 0 ? (bool?)null : (i % 3 == 0 ? false : true);
            
            affectations.Add(new AffectationModel
            {
                Id = i,
                IdDate = i,
                IdEmploye = (i % 12) + 1,
                IdAdresse = (i % 4) + 1,
                IdTypeTransport = (i % 2) + 1,
                IdSite = (i % 7) + 1,
                IdVehicule = i % 3 == 0 ? null : (i % 5) + 1,
                IdHeureTransport = (i % 3) + 1,
                IdType = (i % 2) + 1,
                EstValidee = estValidee,
                Commentaire = estValidee == false ? "Transport annulé par l'employé" : null,
                DateCreation = DateTime.Now.AddDays(-i),
                DateValidation = estValidee.HasValue ? DateTime.Now.AddDays(-i).AddHours(2) : null,
                EstArchive = dateJour < DateTime.Now.AddDays(-30),
                
                // Données de navigation (simulées)
                Employe = new EmployeModel 
                { 
                    Id = (i % 12) + 1,
                    Nom = employes[i % employes.Length].Split(' ')[1],
                    Prenom = employes[i % employes.Length].Split(' ')[0],
                    Matricule = $"EMP{(i % 12) + 1:D3}",
                    Actif = true,
                    IdDepartement = 1,
                    DateInsertion = DateTime.Now
                },
                Site = new SiteModel 
                { 
                    Id = (i % 7) + 1,
                    Nom = sites[i % sites.Length],
                    Actif = true,
                    DateInsertion = DateTime.Now
                },
                Adresse = new AdresseEmployeModel
                {
                    Id = (i % 4) + 1,
                    IdEmploye = (i % 12) + 1,
                    Adresse = adresses[i % adresses.Length],
                    Actif = true,
                    DateInsertion = DateTime.Now
                },
                TypeTransport = new TypeTransportModel
                {
                    Id = (i % 2) + 1,
                    Libelle = i % 2 == 0 ? "Aller" : "Retour",
                    Actif = true
                },
                HeureTransport = new HeureTransportModel
                {
                    Id = (i % 3) + 1,
                    Heure = i % 3 == 0 ? new TimeSpan(7, 30, 0) : 
                            i % 3 == 1 ? new TimeSpan(12, 0, 0) : 
                            new TimeSpan(18, 0, 0),
                    Libelle = i % 3 == 0 ? "Matin" : i % 3 == 1 ? "Midi" : "Soir",
                    Actif = true,
                    DateInsertion = DateTime.Now
                },
                DateTransport = new DateTransportModel
                {
                    Id = i,
                    DateJour = dateJour,
                    Actif = true
                },
                Vehicule = i % 3 == 0 ? null : new VehiculeModel
                {
                    Id = (i % 5) + 1,
                    Matricule = $"VEH{(i % 5) + 1:D3}",
                    NombrePlaces = 4 + (i % 3),
                    Actif = true,
                    DateInsertion = DateTime.Now
                }
            });
        }

        return affectations;
    }

    public IActionResult Index(int page = 1, int pageSize = 10)
    {
        var allAffectations = GetStaticAffectations();

        // Calculer la pagination
        var totalItems = allAffectations.Count;
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        
        // Valider la page
        page = Math.Max(1, Math.Min(page, totalPages));

        // Récupérer les éléments de la page courante
        var affectations = allAffectations
            .OrderByDescending(a => a.DateCreation)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Préparer le modèle de pagination
        var model = new PaginatedViewModel<AffectationModel>
        {
            Items = affectations,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };

        // Statistiques du tableau de bord (calculées depuis les données)
        ViewBag.TransportsAujourdhui = allAffectations.Count(a => 
            a.DateTransport?.DateJour.Date == DateTime.Today);
        ViewBag.EnCours = allAffectations.Count(a => 
            a.EstValidee == true && !a.EstArchive && 
            a.DateTransport?.DateJour.Date == DateTime.Today);
        ViewBag.Termines = allAffectations.Count(a => 
            a.EstValidee == true && 
            a.DateTransport?.DateJour.Date < DateTime.Today);
        ViewBag.Annules = allAffectations.Count(a => a.EstValidee == false);
        ViewBag.EnAttente = allAffectations.Count(a => 
            a.EstValidee == null && !a.EstArchive);
        ViewBag.ClientsActifs = allAffectations.Select(a => a.IdEmploye).Distinct().Count();

        // Si c'est une requête AJAX, retourner uniquement la vue partielle du tableau
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return PartialView("../Partial/_TableauAffectations", model);
        }

        return View(model);
    }

    // Action pour exporter TOUTES les affectations (pour le PDF complet)
    public IActionResult ExportAllView()
    {
        var allAffectations = GetStaticAffectations();

        var model = new PaginatedViewModel<AffectationModel>
        {
            Items = allAffectations.OrderByDescending(a => a.DateCreation).ToList(),
            CurrentPage = 1,
            PageSize = allAffectations.Count,
            TotalItems = allAffectations.Count,
            TotalPages = 1
        };

        // Retourner la même vue partielle mais avec TOUTES les données
        return PartialView("../Partial/_TableauAffectations", model);
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