using Microsoft.EntityFrameworkCore;
using SimulatedAnnealing.Server.Services.Behavioral;
using SimulatedAnnealing.Server.Services.Creational;
using SimulatedAnnealing.Server.Services.Database;
namespace SimulatedAnnealing.Server.Services.Extensions;
public static class ServiceCollectionExtensions
{
    private static string connectionStringName = "PhdApi";
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PhdApiContext>(options => options.UseSqlServer(configuration.GetConnectionString(connectionStringName)));
        services.AddScoped<CacheService, CacheService>();
        services.AddScoped<StateBuilder, StateBuilder>();
        services.AddScoped<ComplianceService, ComplianceService>();
        services.AddScoped<SimulatedAnnealingService, SimulatedAnnealingService>();
    }
}

