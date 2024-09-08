using SimulatedAnnealing.Calculator;
using SimulatedAnnealing.Database;
using SimulatedAnnealing.Geography;
using SimulatedAnnealing.Legal;
using SimulatedAnnealing.Models;

static async void Main()
{
    //CONFIGURATION
    SimulatedAnnealingContext context = new SimulatedAnnealingContext();
    DbRepository dbRepository = new DbRepository(context);
    Radar radar = new Radar(context);
    Calculator calculator = new Calculator();
    DHondt dhondt = new DHondt();
    ElectoralCodex electoralCodex = new ElectoralCodex();

    State currentState = await dbRepository.GetCurrentStateAsync(); 
   


    double initialSolution = 10.0;
    double initialTemperature = 100.0;
    double coolingRate = 0.99;
    double stepSize = 1.0;
    int maxIterations = 1000;

    //double bestSolution = Optimize(initialSolution, initialTemperature, coolingRate, stepSize, maxIterations);
    //Console.WriteLine($"Najlepsze znalezione rozwiązanie: {bestSolution}");
    //Console.WriteLine($"Wartość funkcji celu: {ObjectiveFunction(bestSolution)}");
}
