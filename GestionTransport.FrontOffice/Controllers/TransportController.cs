using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GestionTransport.FrontOffice.Models;
using GestionTransport.FrontOffice.Models.Affectation;
using GestionTransport.FrontOffice.Models.Utils;
using GestionTransport.FrontOffice.Repositories.Interfaces;

namespace GestionTransport.FrontOffice.Controllers;

public class TransportController : Controller
{
    private readonly ILogger<TransportController> _logger;
    private readonly IAffectationRepository _affectationRepository;
    private readonly IEmployeRepository _employeRepository;
    private readonly IAdresseEmployeRepository _adresseEmployeRepository;
    private readonly ITypeTransportRepository _typeTransportRepository;
    private readonly IHeureTransportRepository _heureTransportRepository;
    private readonly IDateTransportRepository _dateTransportRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IVehiculeRepository _vehiculeRepository;
    private readonly ITypeAffectationRepository _typeAffectationRepository;

    // TODO: remplacer par l'id de l'employé connecté (claims/session)
    private const int CURRENT_EMPLOYE_ID = 1;

    public TransportController(
        ILogger<TransportController> logger,
        IAffectationRepository affectationRepository,
        IEmployeRepository employeRepository,
        IAdresseEmployeRepository adresseEmployeRepository,
        ITypeTransportRepository typeTransportRepository,
        IHeureTransportRepository heureTransportRepository,
        IDateTransportRepository dateTransportRepository,
        ISiteRepository siteRepository,
        IVehiculeRepository vehiculeRepository,
        ITypeAffectationRepository typeAffectationRepository)
    {
        _logger = logger;
        _affectationRepository = affectationRepository;
        _employeRepository = employeRepository;
        _adresseEmployeRepository = adresseEmployeRepository;
        _typeTransportRepository = typeTransportRepository;
        _heureTransportRepository = heureTransportRepository;
        _dateTransportRepository = dateTransportRepository;
        _siteRepository = siteRepository;
        _vehiculeRepository = vehiculeRepository;
        _typeAffectationRepository = typeAffectationRepository;
    }

    private List<AffectationModel> LoadAffectationsEmploye(int employeId)
    {
        var affectations = _affectationRepository.GetByEmploye(employeId);

        // Hydratation des propriétés de navigation pour l'affichage
        var employes = _employeRepository.GetAll().ToDictionary(e => e.Id);
        var adresses = _adresseEmployeRepository.GetAll().ToDictionary(a => a.Id);
        var typesTransport = _typeTransportRepository.GetAll().ToDictionary(t => t.Id);
        var sites = _siteRepository.GetAll().ToDictionary(s => s.Id);
        var vehicules = _vehiculeRepository.GetAll().ToDictionary(v => v.Id);
        var heures = _heureTransportRepository.GetAll().ToDictionary(h => h.Id);
        var dates = _dateTransportRepository.GetAll().ToDictionary(d => d.Id);
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

            if (dates.TryGetValue(affectation.IdDate, out var date))
                affectation.DateTransport = date;

            if (typesAffectation.TryGetValue(affectation.IdType, out var typeAffectation))
                affectation.TypeAffectation = typeAffectation;
        }

        return affectations;
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

    public IActionResult MesTransports(int page = 1, int pageSize = 10)
    {
        var affectations = LoadAffectationsEmploye(CURRENT_EMPLOYE_ID)
            .OrderByDescending(a => a.DateCreation)
            .ToList();

        var totalItems = affectations.Count;
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        page = Math.Max(1, totalPages == 0 ? 1 : Math.Min(page, totalPages));

        var pageItems = affectations
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var model = new PaginatedViewModel<AffectationModel>
        {
            Items = pageItems,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };

        return View(model);
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
