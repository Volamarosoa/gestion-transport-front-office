using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GestionTransport.FrontOffice.Models;
using GestionTransport.FrontOffice.Extensions;
using GestionTransport.FrontOffice.Models.Utils;
using GestionTransport.FrontOffice.Repositories.Interfaces;

namespace GestionTransport.FrontOffice.Controllers;

[Authorize]
public class CircuitController : Controller
{
    private readonly ILogger<CircuitController> _logger;
    private readonly IAffectationRepository _affectationRepository;

    public CircuitController(
        ILogger<CircuitController> logger,
        IAffectationRepository affectationRepository)
    {
        _logger = logger;
        _affectationRepository = affectationRepository;
    }

    public IActionResult MonCircuit()
    {
        var employeId = User.GetEmployeId();
        var allAffectationsToday = _affectationRepository.GetValidatedAffectationsToday();

        var mesVehiculesIds = allAffectationsToday
            .Where(a => a.IdEmploye == employeId && a.IdVehicule.HasValue)
            .Select(a => a.IdVehicule!.Value)
            .Distinct()
            .ToHashSet();

        var model = new CircuitJourViewModel
        {
            Date = DateTime.Today,
            Vehicules = allAffectationsToday
                .Where(a => a.IdVehicule.HasValue && mesVehiculesIds.Contains(a.IdVehicule.Value))
                .GroupBy(a => a.IdVehicule!.Value)
                .Select(g =>
                {
                    var first = g.First();
                    var site = first.Site;

                    var personnes = g
                        .Select(a => new PersonneCircuitViewModel
                        {
                            EmployeId = a.IdEmploye,
                            NomComplet = a.Employe?.NomComplet() ?? "(Employe)",
                            Matricule = a.Employe?.Matricule,
                            Adresse = a.Adresse?.Adresse,
                            Latitude = a.Adresse?.Latitude,
                            Longitude = a.Adresse?.Longitude,
                            Heure = a.HeureTransport?.FormatHeure(),
                            TypeTransport = a.TypeTransport?.Libelle,
                            DistanceKmVersSite = ComputeDistanceKm(
                                site?.Latitude,
                                site?.Longitude,
                                a.Adresse?.Latitude,
                                a.Adresse?.Longitude)
                        })
                        .OrderBy(p => p.DistanceKmVersSite ?? double.MaxValue)
                        .ThenBy(p => p.NomComplet)
                        .ToList();

                    return new VehiculeCircuitViewModel
                    {
                        VehiculeId = g.Key,
                        VehiculeLabel = first.Vehicule?.Matricule ?? $"Vehicule #{g.Key}",
                        NombrePlaces = first.Vehicule?.NombrePlaces,
                        Site = site == null
                            ? null
                            : new SitePointViewModel
                            {
                                SiteId = site.Id,
                                Nom = site.Nom,
                                Latitude = site.Latitude,
                                Longitude = site.Longitude
                            },
                        Personnes = personnes
                    };
                })
                .OrderBy(v => v.VehiculeLabel)
                .ToList()
        };

        return View(model);
    }    

    private static double? ComputeDistanceKm(decimal? lat1, decimal? lon1, decimal? lat2, decimal? lon2)
    {
        if (!lat1.HasValue || !lon1.HasValue || !lat2.HasValue || !lon2.HasValue)
        {
            return null;
        }

        const double earthRadiusKm = 6371.0;
        double ToRad(double angle) => angle * Math.PI / 180.0;

        var dLat = ToRad((double)(lat2.Value - lat1.Value));
        var dLon = ToRad((double)(lon2.Value - lon1.Value));
        var originLat = ToRad((double)lat1.Value);
        var destLat = ToRad((double)lat2.Value);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(originLat) * Math.Cos(destLat) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadiusKm * c;
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
