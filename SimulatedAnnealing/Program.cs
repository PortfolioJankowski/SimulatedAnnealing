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

static async void Main()
{
    ServiceProviderImplementation DI = new ServiceProviderImplementation();
    var serviceProvider = DI.GetServices();
    var stateBuilder = serviceProvider.GetService<StateBuilder>();
    var dbRepository = serviceProvider.GetService<DbRepository>();
    var radar = serviceProvider.GetService<Radar>();
    Paint.Start();
    State initialState = stateBuilder.Build(isInitialState: true);
    Console.WriteLine($"Initial STATE: {initialState.Indicator.Score}");

    Algorithm algorithm = new Algorithm(radar);
    State optimalState = algorithm.Optimize(initialState);
    Paint.ShowResults(initialState,optimalState);



}

Main();