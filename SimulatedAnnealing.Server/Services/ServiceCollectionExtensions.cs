using Microsoft.EntityFrameworkCore;
using SimulatedAnnealing.Server.Models;

namespace SimulatedAnnealing.Server.Services
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PhdApiContext>(options => options.UseSqlServer(configuration.GetConnectionString("PhdApi")));
        }
    }
}
