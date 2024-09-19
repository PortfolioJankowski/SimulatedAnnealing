using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimulatedAnnealing.Services.Builders;
using SimulatedAnnealing.Services.Database;
using SimulatedAnnealing.Services.Geography;
using SimulatedAnnealing.Services.Legal;
using SimulatedAnnealing.Services.Math;
using SimulatedAnnealing.Services.Painter;
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
