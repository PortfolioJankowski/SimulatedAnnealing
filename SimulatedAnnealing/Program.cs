using Microsoft.Extensions.DependencyInjection;
using SimulatedAnnealing.Models;
using SimulatedAnnealing.Services.Database;
using SimulatedAnnealing.Services.Geography;
using SimulatedAnnealing.Services.Legal;
using SimulatedAnnealing.Services.Math;
using SimulatedAnnealing.Services.Painter;
using SimulatedAnnealing;
using SimulatedAnnealing.Services.Builders;
using SimulatedAnnealing.Services.Config;


static void Main()
{
    ServiceProviderImplementation DI = new ServiceProviderImplementation();
    var serviceProvider = DI.GetServices();
    var stateBuilder = serviceProvider.GetService<StateBuilder>()!;
    
    Paint.Start();
    State initialState = stateBuilder.Build(isInitialState: true);
    Console.WriteLine($"Initial STATE: {initialState.Indicator!.Score}");

    Algorithm algorithm = new Algorithm(serviceProvider.GetService<Radar>()!, serviceProvider.GetService<Predictor>()!, 
                                        serviceProvider.GetService<ElectoralCodex>()!, serviceProvider.GetService<DbRepository>()!);
    State optimalState = algorithm.Optimize(initialState);
    
    var dbRepository = serviceProvider.GetService<DbRepository>()!;
    dbRepository.SaveStateAsync(optimalState, initialState);


     


}

Main();