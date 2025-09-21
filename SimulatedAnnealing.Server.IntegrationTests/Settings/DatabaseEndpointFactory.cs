using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimulatedAnnealing.Server.Services.Authentication;
using SimulatedAnnealing.Server.Services.Database;

namespace SimulatedAnnealing.Server.IntegrationTests;

public class DatabaseEndpointFactory : WebApplicationFactory<Program>
{
    private string _testConnectionString;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Configure application configuration to fetch the test connection string
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var configBuilder = config.Build();
            _testConnectionString = configBuilder.GetConnectionString("PhdApi_Test");
        });

        // Configure services for integration testing
        builder.ConfigureTestServices(services =>
        {
            // Remove the existing DbContext configuration if any
            var descriptor = services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<PhdApiContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add the test DbContext with the test connection string
            services.AddDbContext<PhdApiContext>(options =>
            {
                options.UseSqlServer(_testConnectionString);
            });

            // Register other dependencies as needed
            services.AddScoped<IDbRepository, DbRepository>();
            services.AddScoped<IUserService, UserService>();
        });
    }
}


