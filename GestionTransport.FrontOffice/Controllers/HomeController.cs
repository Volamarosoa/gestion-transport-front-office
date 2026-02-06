using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GestionTransport.FrontOffice.Models;
using GestionTransport.FrontOffice.Models.Affectation;
using GestionTransport.FrontOffice.Models.Employe;
using GestionTransport.FrontOffice.Models.Transport;
using GestionTransport.FrontOffice.Models.Utils;
using Microsoft.AspNetCore.Mvc.Rendering;

using System.Data;
using GestionTransport.FrontOffice.Services;

namespace GestionTransport.FrontOffice.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly CsvImportService _csvImportService; 

    public HomeController(ILogger<HomeController> logger, CsvImportService csvImport)
    {
        _logger = logger;
        _csvImportService = csvImport;
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

    public IActionResult ImportCsv()
    {
        return View();
    }

    // ⭐ ACTION 2 : Télécharger un exemple de CSV
    public IActionResult DownloadCsvTemplate()
    {
        var csvBytes = _csvImportService.GenerateExampleTemplate();
        return File(csvBytes, "text/csv", "Exemple_Affectations.csv");
    }

    // ⭐ ACTION 3 : Analyser le CSV uploadé
    [HttpPost]
    public async Task<IActionResult> AnalyzeCsv(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Veuillez sélectionner un fichier CSV.";
            return RedirectToAction("ImportCsv");
        }

        if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            TempData["Error"] = "Le fichier doit être au format CSV.";
            return RedirectToAction("ImportCsv");
        }

        try
        {
            using (var stream = file.OpenReadStream())
            {
                var structure = _csvImportService.AnalyzeCsv(stream, ";");
                
                // Sauvegarder le fichier temporairement
                var tempFileName = $"{Guid.NewGuid()}.csv";
                var tempPath = Path.Combine(Path.GetTempPath(), tempFileName);
                
                using (var fileStream = new FileStream(tempPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                ViewBag.TempFileName = tempFileName;
                
                return View("MapCsvColumns", structure);
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Erreur lors de l'analyse : {ex.Message}";
            return RedirectToAction("ImportCsv");
        }
    }

    // ⭐ ACTION 4 : Traiter l'import avec le mapping
    [HttpPost]
    public IActionResult ProcessCsvImport(string tempFileName, Dictionary<string, string> columnMapping)
    {
        try
        {
            var tempPath = Path.Combine(Path.GetTempPath(), tempFileName);
            
            if (!System.IO.File.Exists(tempPath))
            {
                TempData["Error"] = "Fichier temporaire introuvable.";
                return RedirectToAction("ImportCsv");
            }

            using (var stream = new FileStream(tempPath, FileMode.Open))
            {
                var dataTable = _csvImportService.ReadCsvToDataTable(stream, ";");
                
                var importedCount = 0;
                var importedData = new List<string>();

                foreach (DataRow row in dataTable.Rows)
                {
                    var rowData = new List<string>();
                    
                    foreach (var mapping in columnMapping)
                    {
                        var csvColumn = mapping.Key;
                        var targetField = mapping.Value;
                        
                        if (!string.IsNullOrEmpty(targetField))
                        {
                            var value = row[csvColumn].ToString();
                            rowData.Add($"{targetField}: {value}");
                        }
                    }
                    
                    importedData.Add($"Ligne {importedCount + 1}: {string.Join(", ", rowData)}");
                    importedCount++;
                }

                // Nettoyer le fichier temporaire
                System.IO.File.Delete(tempPath);

                TempData["Success"] = $"{importedCount} ligne(s) importée(s) avec succès !";
                TempData["ImportedData"] = string.Join("<br/>", importedData);
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Erreur lors de l'import : {ex.Message}";
        }

        return RedirectToAction("ImportResult");
    }

    // ⭐ ACTION 5 : Afficher le résultat de l'import
    public IActionResult ImportResult()
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