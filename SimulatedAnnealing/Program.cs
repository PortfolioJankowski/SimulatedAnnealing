using Microsoft.Extensions.DependencyInjection;
using SimulatedAnnealing.Models;
using SimulatedAnnealing.Services.Database;
using SimulatedAnnealing.Services.Geography;
using SimulatedAnnealing.Services.Legal;
using SimulatedAnnealing.Services;
using SimulatedAnnealing.Services.Math;
using SimulatedAnnealing.Services.Painter;
using SimulatedAnnealing;
using SimulatedAnnealing.Services.Builders;

static async void Main()
{
    //configuration
    ServiceProviderImplementation DI = new ServiceProviderImplementation();
    var serviceProvider = DI.GetServices();

    var configurationBuilder = serviceProvider.GetService<ConfigurationBuilder>();
    var stateBuilder = serviceProvider.GetService<StateBuilder>();
    var configuration = configurationBuilder!.Build();
    var dbRepository = serviceProvider.GetService<DbRepository>();


    //initialState
    Paint.Start();
    State initialState = stateBuilder.Build(isInitialState: true);

    Algorithm algorithm = new Algorithm();
    

    //double bestSolution = Optimize(initialSolution, initialTemperature, coolingRate, stepSize, maxIterations);
    //Console.WriteLine($"Najlepsze znalezione rozwiązanie: {bestSolution}");
    //Console.WriteLine($"Wartość funkcji celu: {ObjectiveFunction(bestSolution)}");
}

Main();