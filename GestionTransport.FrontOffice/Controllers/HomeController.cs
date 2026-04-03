using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GestionTransport.FrontOffice.Models;
using GestionTransport.FrontOffice.Models.Affectation;
using GestionTransport.FrontOffice.Models.Employe;
using GestionTransport.FrontOffice.Models.Transport;
using GestionTransport.FrontOffice.Models.Utils;

using System.Data;
using GestionTransport.FrontOffice.Extensions;
using GestionTransport.FrontOffice.Services;
using GestionTransport.FrontOffice.Repositories.Interfaces;

namespace GestionTransport.FrontOffice.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly CsvImportService _csvImportService;
    private readonly IAffectationRepository _affectationRepository;
    private readonly IEmployeRepository _employeRepository;
    private readonly IAdresseEmployeRepository _adresseEmployeRepository;
    private readonly ITypeTransportRepository _typeTransportRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IVehiculeRepository _vehiculeRepository;
    private readonly IHeureTransportRepository _heureTransportRepository;
    private readonly ITypeAffectationRepository _typeAffectationRepository;

    public HomeController(
        ILogger<HomeController> logger,
        CsvImportService csvImport,
        IAffectationRepository affectationRepository,
        IEmployeRepository employeRepository,
        IAdresseEmployeRepository adresseEmployeRepository,
        ITypeTransportRepository typeTransportRepository,
        ISiteRepository siteRepository,
        IVehiculeRepository vehiculeRepository,
        IHeureTransportRepository heureTransportRepository,
        ITypeAffectationRepository typeAffectationRepository)
    {
        _logger = logger;
        _csvImportService = csvImport;
        _affectationRepository = affectationRepository;
        _employeRepository = employeRepository;
        _adresseEmployeRepository = adresseEmployeRepository;
        _typeTransportRepository = typeTransportRepository;
        _siteRepository = siteRepository;
        _vehiculeRepository = vehiculeRepository;
        _heureTransportRepository = heureTransportRepository;
        _typeAffectationRepository = typeAffectationRepository;
    }

    // Charger les affectations de l'employé connecté avec navigation
    private List<AffectationModel> LoadMesAffectationsAvecNavigation()
    {
        var affectations = _affectationRepository.GetByEmploye(User.GetEmployeId());

        var employes = _employeRepository.GetAll().ToDictionary(e => e.Id);
        var adresses = _adresseEmployeRepository.GetAll().ToDictionary(a => a.Id);
        var typesTransport = _typeTransportRepository.GetAll().ToDictionary(t => t.Id);
        var sites = _siteRepository.GetAll().ToDictionary(s => s.Id);
        var vehicules = _vehiculeRepository.GetAll().ToDictionary(v => v.Id);
        var heures = _heureTransportRepository.GetAll().ToDictionary(h => h.Id);
        var typesAffectation = _typeAffectationRepository.GetAll().ToDictionary(t => t.Id);

        foreach (var affectation in affectations)
        {
            if (employes.TryGetValue(affectation.IdEmploye, out var employe))
                affectation.Employe = employe;

            if (adresses.TryGetValue(affectation.IdAdresse, out var adresse))
                affectation.Adresse = adresse;

            if (typesTransport.TryGetValue(affectation.IdTypeTransport, out var typeTransport))
                affectation.TypeTransport = typeTransport;

            if (sites.TryGetValue(affectation.IdSite, out var site))
                affectation.Site = site;

            if (affectation.IdVehicule.HasValue && vehicules.TryGetValue(affectation.IdVehicule.Value, out var vehicule))
                affectation.Vehicule = vehicule;

            if (heures.TryGetValue(affectation.IdHeureTransport, out var heure))
                affectation.HeureTransport = heure;

            if (typesAffectation.TryGetValue(affectation.IdType, out var typeAffectation))
                affectation.TypeAffectation = typeAffectation;
        }

        return affectations;
    }

    // Construire la liste du jour : afficher le(s) véhicule(s) assigné(s) à l'employé aujourd'hui
    private List<VehiculeJourViewModel> BuildMesVehiculesAujourdhui()
    {
        var aujourdHui = DateTime.Today;
        var mesAffectations = LoadMesAffectationsAvecNavigation()
            .Where(a => a.DateTransport?.Date == aujourdHui)
            .ToList();

        var groupes = mesAffectations
            .GroupBy(a => a.Vehicule?.Id)
            .Select(g =>
            {
                var vehicule = g.First().Vehicule;
                return new VehiculeJourViewModel
                {
                    VehiculeId = vehicule?.Id,
                    VehiculeLabel = vehicule?.Matricule ?? "Aucun véhicule assigné",
                    NombrePlaces = vehicule?.NombrePlaces,
                    Employes = g
                        .OrderBy(a => a.HeureTransport?.Heure)
                        .Select(a => new EmployeTransportItemViewModel
                        {
                            NomComplet = a.Employe?.NomComplet() ?? "(Employé)",
                            Matricule = a.Employe?.Matricule,
                            Adresse = a.Adresse?.Adresse,
                            Site = a.Site?.Nom,
                            TypeTransport = a.TypeTransport?.Libelle,
                            Heure = a.HeureTransport?.FormatHeure(),
                            Commentaire = a.Commentaire
                        })
                        .ToList()
                };
            })
            .OrderBy(v => v.VehiculeLabel)
            .ToList();

        return groupes;
    }

    private List<VehiculeJourViewModel> BuildTransportsPublicAujourdhui()
    {
        var affectations = _affectationRepository.GetValidatedAffectationsToday();

        var groupes = affectations
            .GroupBy(a => a.Vehicule?.Id)
            .Select(g =>
            {
                var vehicule = g.First().Vehicule;
                return new VehiculeJourViewModel
                {
                    VehiculeId = vehicule?.Id,
                    VehiculeLabel = vehicule?.Matricule ?? "Aucun véhicule assigné",
                    NombrePlaces = vehicule?.NombrePlaces,
                    Employes = g
                        .OrderBy(a => a.HeureTransport?.Heure)
                        .Select(a => new EmployeTransportItemViewModel
                        {
                            NomComplet = a.Employe?.NomComplet() ?? "(Employé)",
                            Matricule = a.Employe?.Matricule,
                            Adresse = a.Adresse?.Adresse,
                            Site = a.Site?.Nom,
                            TypeTransport = a.TypeTransport?.Libelle,
                            Heure = a.HeureTransport?.FormatHeure(),
                            Commentaire = a.Commentaire
                        })
                        .ToList()
                };
            })
            .OrderBy(v => v.VehiculeLabel)
            .ToList();

        return groupes;
    }

    // Page d'accueil : mes affectations avec pagination
    public IActionResult Index(int page = 1, int pageSize = 10)
    {
        var allAffectations = LoadMesAffectationsAvecNavigation();

        var totalItems = allAffectations.Count;
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        page = Math.Max(1, totalPages == 0 ? 1 : Math.Min(page, totalPages));

        var affectations = allAffectations
            .OrderByDescending(a => a.DateCreation)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var model = new PaginatedViewModel<AffectationModel>
        {
            Items = affectations,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };

        // Statistiques personnelles de l'employé
        ViewBag.TransportsAujourdhui = allAffectations.Count(a =>
            a.DateTransport?.Date == DateTime.Today);
        ViewBag.EnCours = allAffectations.Count(a =>
            a.EstValidee == true && !a.EstArchive &&
            a.DateTransport?.Date == DateTime.Today);
        ViewBag.Termines = allAffectations.Count(a =>
            a.EstValidee == true &&
            a.DateTransport?.Date < DateTime.Today);
        ViewBag.Annules = allAffectations.Count(a => a.EstValidee == false);
        ViewBag.EnAttente = allAffectations.Count(a =>
            a.EstValidee == null && !a.EstArchive);

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return PartialView("../Partial/_TableauAffectations", model);
        }

        return View(model);
    }

    // Exporter toutes mes affectations (pour le PDF)
    public IActionResult ExportAllView()
    {
        var allAffectations = LoadMesAffectationsAvecNavigation();

        var model = new PaginatedViewModel<AffectationModel>
        {
            Items = allAffectations.OrderByDescending(a => a.DateCreation).ToList(),
            CurrentPage = 1,
            PageSize = allAffectations.Count,
            TotalItems = allAffectations.Count,
            TotalPages = 1
        };

        return PartialView("../Partial/_TableauAffectations", model);
    }

    // Page bienvenue : mes véhicules assignés aujourd'hui
    [AllowAnonymous]
    public IActionResult Bienvenue(int? vehiculeId)
    {
        var allVehicules = BuildTransportsPublicAujourdhui();
        ViewBag.VehiculesFilter = allVehicules
            .OrderBy(v => v.VehiculeLabel)
            .ToList();
        ViewBag.SelectedVehiculeId = vehiculeId;

        var model = vehiculeId.HasValue
            ? allVehicules.Where(v => v.VehiculeId == vehiculeId.Value).ToList()
            : allVehicules;

        return View(model);
    }

    // Export PDF du jour
    public IActionResult ExportJourPdf()
    {
        var model = BuildMesVehiculesAujourdhui();
        return View(model);
    }

    public IActionResult ImportCsv()
    {
        return View();
    }

    // Télécharger un exemple de CSV
    public IActionResult DownloadCsvTemplate()
    {
        var csvBytes = _csvImportService.GenerateExampleTemplate();
        return File(csvBytes, "text/csv", "Exemple_Affectations.csv");
    }

    // Analyser le CSV uploadé
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

    // Traiter l'import avec le mapping
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

    // Résultat de l'import
    public IActionResult ImportResult()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
