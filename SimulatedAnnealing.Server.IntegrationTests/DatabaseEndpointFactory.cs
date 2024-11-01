using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimulatedAnnealing.Server.Models.Algorithm.Fixed;
using SimulatedAnnealing.Server.Models.Authentication;
using SimulatedAnnealing.Server.Services.Authentication;
using SimulatedAnnealing.Server.Services.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedAnnealing.Server.IntegrationTests;

public class DatabaseEndpointFactory : WebApplicationFactory<Program>
{
   protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var configuration = config.Build();
            var testConnectionString = configuration.GetConnectionString("PhdApi_Test"); 

            builder.ConfigureServices(services =>
            {
                //DI for tests
                services.AddScoped<IUserService, UserService>();
                // Ensure that the necessary Identity services are configured
                services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<PhdApiContext>()
                .AddDefaultTokenProviders();

                // Register your services
                services.AddScoped<IUserService, UserService>();
                services.AddScoped<ITokenService, TokenService>(); // Ensure TokenService is registered


                //Replacing Prod DB with TestDB
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<PhdApiContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<PhdApiContext>(options =>
                    options.UseSqlServer(testConnectionString));
            });
        });
    }
    //Method for fetching services in the purpose of tests
    public async Task<TService> GetScopedService<TService>()
    {
        using var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<TService>();
    }
}

