using GestionTransport.FrontOffice.Repositories;

namespace GestionTransport.FrontOffice.Services;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Services
        services.AddScoped<IDatabaseService, DatabaseService>();
            
        // Repositories
        services.AddScoped<ICompteRepository, CompteRepository>();
            
        return services;
    }
}