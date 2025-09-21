using Microsoft.Extensions.DependencyInjection;
using SimulatedAnnealing.Services.Builders;
using SimulatedAnnealing.Services.Database;
using SimulatedAnnealing.Services.Geography;
using SimulatedAnnealing.Services.Legal;
using SimulatedAnnealing.Services.Math;
namespace SimulatedAnnealing.Services.Config
{
    public class ServiceProviderImplementation
    {
        public ServiceProvider GetServices()
        {
            return new ServiceCollection()
              .AddSingleton<Models.SimulatedAnnealingContext>()
              .AddSingleton<DbRepository>()
              .AddSingleton<Radar>()
              .AddSingleton<Predictor>()
              .AddSingleton<ElectoralCodex>()
              .AddSingleton<StateBuilder>()
              .BuildServiceProvider();
        }
    }
}
