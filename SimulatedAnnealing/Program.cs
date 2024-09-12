using Microsoft.Extensions.DependencyInjection;
using SimulatedAnnealing.Models;
using SimulatedAnnealing.Services.Database;
using SimulatedAnnealing.Services.Geography;
using SimulatedAnnealing.Services.Legal;
using SimulatedAnnealing.Services;
using SimulatedAnnealing.Services.Math;
using SimulatedAnnealing.Services.Painter;

static async void Main()
{
    //configuration
    ServiceProviderImplementation DI = new ServiceProviderImplementation();
    var serviceProvider = DI.GetServices();

    var configurationBuilder = serviceProvider.GetService<ConfigurationBuilder>();
    var configuration = configurationBuilder!.Build();
    var dbRepository = serviceProvider.GetService<DbRepository>();


    //initialState
    Paint.Start();
    State initialState = dbRepository!.GetCurrentState();

    

    
    
    

    

    //double bestSolution = Optimize(initialSolution, initialTemperature, coolingRate, stepSize, maxIterations);
    //Console.WriteLine($"Najlepsze znalezione rozwiązanie: {bestSolution}");
    //Console.WriteLine($"Wartość funkcji celu: {ObjectiveFunction(bestSolution)}");
}

Main();