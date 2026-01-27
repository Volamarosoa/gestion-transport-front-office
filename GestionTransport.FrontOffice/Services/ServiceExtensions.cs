using GestionTransport.FrontOffice.Repositories;
using GestionTransport.FrontOffice.Repositories.Interfaces;

namespace GestionTransport.FrontOffice.Services
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Database service
            services.AddScoped<IDatabaseService, DatabaseService>();

            // Repositories
            // services.AddScoped<ICompteRepository, CompteRepository>(); // ← SUPPRIMER CETTE LIGNE
            
            services.AddScoped<IEmployeRepository, EmployeRepository>();
            services.AddScoped<IDepartementRepository, DepartementRepository>();
            services.AddScoped<ISiteRepository, SiteRepository>();
            services.AddScoped<IVehiculeRepository, VehiculeRepository>();
            services.AddScoped<ITypeTransportRepository, TypeTransportRepository>();
            services.AddScoped<IHeureTransportRepository, HeureTransportRepository>();
            services.AddScoped<IDateTransportRepository, DateTransportRepository>();
            services.AddScoped<IAdresseEmployeRepository, AdresseEmployeRepository>();
            services.AddScoped<ITypeAffectationRepository, TypeAffectationRepository>();
            services.AddScoped<IAffectationRepository, AffectationRepository>();

            return services;
        }

        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<AffectationArchivageService>();
            services.AddScoped<CsvImportService>();
            
            return services;
        }
    }
}