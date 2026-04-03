using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GestionTransport.FrontOffice.Models;
using GestionTransport.FrontOffice.Models.Affectation;
using GestionTransport.FrontOffice.Models.Utils;
using GestionTransport.FrontOffice.Extensions;
using GestionTransport.FrontOffice.Repositories.Interfaces;
using GestionTransport.FrontOffice.Services;

namespace GestionTransport.FrontOffice.Controllers;

[Authorize]
public class TransportController : Controller
{
    private readonly ILogger<TransportController> _logger;
    private readonly IAffectationRepository _affectationRepository;
    private readonly IEmployeRepository _employeRepository;
    private readonly IAdresseEmployeRepository _adresseEmployeRepository;
    private readonly ITypeTransportRepository _typeTransportRepository;
    private readonly IHeureTransportRepository _heureTransportRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IVehiculeRepository _vehiculeRepository;
    private readonly ITypeAffectationRepository _typeAffectationRepository;
    private readonly AffectationService _affectationService;

    public TransportController(
        ILogger<TransportController> logger,
        IAffectationRepository affectationRepository,
        IEmployeRepository employeRepository,
        IAdresseEmployeRepository adresseEmployeRepository,
        ITypeTransportRepository typeTransportRepository,
        IHeureTransportRepository heureTransportRepository,
        ISiteRepository siteRepository,
        IVehiculeRepository vehiculeRepository,
        ITypeAffectationRepository typeAffectationRepository,
        AffectationService affectationService)
    {
        _logger = logger;
        _affectationRepository = affectationRepository;
        _employeRepository = employeRepository;
        _adresseEmployeRepository = adresseEmployeRepository;
        _typeTransportRepository = typeTransportRepository;
        _heureTransportRepository = heureTransportRepository;
        _siteRepository = siteRepository;
        _vehiculeRepository = vehiculeRepository;
        _typeAffectationRepository = typeAffectationRepository;
        _affectationService = affectationService;
    }

    private int ResolveSiteId()
    {
        var site = _siteRepository.GetActifs().FirstOrDefault();
        if (site == null)
            throw new InvalidOperationException("Aucun site actif n'est configuré.");

        return site.Id;
    }

    private int ResolveAdresseId(int employeId, string? adresseDepart)
    {
        if (!string.IsNullOrWhiteSpace(adresseDepart))
        {
            var nouvelleAdresse = new Models.Employe.AdresseEmployeModel
            {
                IdEmploye = employeId,
                Adresse = adresseDepart.Trim(),
                EstPrincipale = false,
                Actif = true,
                DateInsertion = DateTime.Now
            };

            return _adresseEmployeRepository.Create(nouvelleAdresse);
        }

        var adressePrincipale = _adresseEmployeRepository.GetAdressePrincipale(employeId)
            ?? _adresseEmployeRepository.GetActifsByEmploye(employeId).FirstOrDefault();

        if (adressePrincipale == null)
            throw new InvalidOperationException("Aucune adresse active n'est configurée pour votre profil.");

        return adressePrincipale.Id;
    }

    private int ResolveHeureTransportId(TimeSpan? heureSouhaitee)
    {
        if (heureSouhaitee.HasValue)
        {
            var exacte = _heureTransportRepository.GetByHeure(heureSouhaitee.Value);
            if (exacte != null)
                return exacte.Id;
        }

        var heureDefaut = _heureTransportRepository.GetActifs()
            .OrderBy(h => h.Heure)
            .FirstOrDefault();

        if (heureDefaut == null)
            throw new InvalidOperationException("Aucun créneau horaire actif n'est configuré.");

        return heureDefaut.Id;
    }

    private void CreateDemande(DateTime date, int employeId, int adresseId, int idTypeTransport, int siteId, int heureId, string? commentaire)
    {
        _affectationService.Create(
            date,
            employeId,
            adresseId,
            idTypeTransport,
            siteId,
            heureId,
            commentaire);
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

    public IActionResult DemanderTransport()
    {
        var employeId = User.GetEmployeId();
        var adresseParDefaut = _adresseEmployeRepository.GetAdressePrincipale(employeId)
            ?? _adresseEmployeRepository.GetActifsByEmploye(employeId).FirstOrDefault();
        ViewBag.AdresseParDefaut = adresseParDefaut?.Adresse;

        return View(new TransportModel { Date = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DemanderTransport(TransportModel model)
    {
        var employeId = User.GetEmployeId();
        var adresseParDefaut = _adresseEmployeRepository.GetAdressePrincipale(employeId)
            ?? _adresseEmployeRepository.GetActifsByEmploye(employeId).FirstOrDefault();
        ViewBag.AdresseParDefaut = adresseParDefaut?.Adresse;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (model.Date.Date < DateTime.Today)
        {
            ModelState.AddModelError(nameof(model.Date), "La date de transport ne peut pas etre inferieure a la date du jour.");
            return View(model);
        }

        try
        {
            var siteId = ResolveSiteId();
            var adresseId = ResolveAdresseId(employeId, model.AdresseDepart);
            var heureId = ResolveHeureTransportId(model.HeureSouhaitee);

            var type = (model.Type ?? string.Empty).Trim().ToLowerInvariant();
            if (type == "allerretour")
            {
                var aller = _typeTransportRepository.GetByLibelle("Aller");
                var retour = _typeTransportRepository.GetByLibelle("Retour");

                if (aller == null || retour == null)
                    throw new InvalidOperationException("Les types de transport Aller/Retour ne sont pas configurés.");

                CreateDemande(model.Date, employeId, adresseId, aller.Id, siteId, heureId, model.Commentaire);
                CreateDemande(model.Date, employeId, adresseId, retour.Id, siteId, heureId, model.Commentaire);
            }
            else
            {
                var typeTransport = _typeTransportRepository.GetByLibelle(model.Type);
                if (typeTransport == null)
                    throw new InvalidOperationException("Le type de transport sélectionné est introuvable.");

                CreateDemande(model.Date, employeId, adresseId, typeTransport.Id, siteId, heureId, model.Commentaire);
            }

            TempData["Success"] = "Votre demande de transport a été soumise avec succès!";
            return RedirectToAction("MesTransports");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    public IActionResult MesTransports(int page = 1, int pageSize = 10)
    {
        var affectations = LoadAffectationsEmploye(User.GetEmployeId())
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
        var affectation = LoadAffectationsEmploye(User.GetEmployeId())
            .FirstOrDefault(a => a.Id == id);

        if (affectation == null)
        {
            return NotFound();
        }

        return View(affectation);
    }

    // POST: Annuler une demande
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AnnulerTransport(int id)
    {
        try
        {
            _affectationService.Annuler(id, User.GetEmployeId());
            TempData["Success"] = "Demande annulée avec succès!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("MesTransports");
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
