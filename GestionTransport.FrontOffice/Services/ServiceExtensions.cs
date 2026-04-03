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
            services.AddScoped<IHistoriqueAffectationRepository, HistoriqueAffectationRepository>();
            services.AddScoped<IAuthentificationRepository, AuthentificationRepository>();

            return services;
        }

        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<AdresseEmployeService>();
            services.AddScoped<HeureTransportService>();
            services.AddScoped<AffectationService>();
            services.AddScoped<CsvImportService>();
            
            return services;
        }
    }
}
