using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimulatedAnnealing.Server.Models.Authentication;
using SimulatedAnnealing.Server.Services.Authentication;
using SimulatedAnnealing.Server.Services.Behavioral;
using SimulatedAnnealing.Server.Services.Creational;
using SimulatedAnnealing.Server.Services.Database;
using SimulatedAnnealing.Server.Services.Middlewares;

namespace SimulatedAnnealing.Server.Services.Extensions;

public static class ServiceCollectionExtensions
{
    private static string _connectionStringName = "PhdApi";
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PhdApiContext>(options => options.UseSqlServer(configuration.GetConnectionString(_connectionStringName)));
        services.AddScoped<IDbRepository, DbRepository>();
        services.AddScoped<StateBuilder, StateBuilder>();
        services.AddScoped<ComplianceService, ComplianceService>();
        services.AddScoped<SimulatedAnnealingService, SimulatedAnnealingService>();
        services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireLowercase = true;
            options.Password.RequiredLength = 5;
        })
            .AddEntityFrameworkStores<PhdApiContext>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserService, UserService>();
        services.AddTransient<AuthMiddleware>();

        // Initialize the Cache
        var serviceProvider = services.BuildServiceProvider();
        var config = serviceProvider.GetRequiredService<IConfiguration>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>(); 
        var context = serviceProvider.GetRequiredService<PhdApiContext>();

        Cache.Initialize(config, logger, context);
    }
}

