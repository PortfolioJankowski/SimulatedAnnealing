using Microsoft.Extensions.Caching.Distributed;
using SimulatedAnnealing.Server.Models.Algorithm.Fixed;
using SimulatedAnnealing.Server.Models.Algorithm.Variable;
using SimulatedAnnealing.Server.Models.DTOs;
using SimulatedAnnealing.Server.Models.Requests;
using SimulatedAnnealing.Server.Services.Algorithm;
using SimulatedAnnealing.Server.Services.Database;

namespace SimulatedAnnealing.Server.Services.Behavioral;
public class SimulatedAnnealingService
{
    private readonly ComplianceService _complianceService;
    private readonly IDbRepository _dbRepository;
    private readonly VoivodeshipStateBuilder _stateBuilder;
    private readonly Random _random;
    
    public SimulatedAnnealingService(ComplianceService complianceService, IDbRepository dbRepository, VoivodeshipStateBuilder stateBuilder)
    {
        _complianceService = complianceService;
        _dbRepository = dbRepository;
        _stateBuilder = stateBuilder;
        _random = new Random();
    }
    public async Task<VoivodeshipState> Optimize(OptimizeLocalDistrictsRequest request)
    {
        var currentSolution = _stateBuilder
            .SetVoivodeship(request.DistrictInformation).Result
            .CalculateInhabitants()
            .CalculateVoievodianshipSeatsAmount()
            .CalculatePopulationIndex()
            .CalculateDistrictResults()
            .Build();
            
        var algorithmConfiguration = request.AlgorithmConfiguration;
        var bestSolution = currentSolution;
        var bestObjective = currentSolution.Indicator!.Score;

        int neighboursAmount = 5;
        var currentObjective = currentSolution.Indicator!.Score;
        var bestRandomSolutionObjective = 0d;

        for (int i = 0; i < algorithmConfiguration.MaxIterations; i++)
        {
            var randomStates = GenerateRandomSolutions(currentSolution, algorithmConfiguration.StepSize, neighboursAmount);
            var bestRandomSolution = GetBestRandomSolution(randomStates);
            bestRandomSolutionObjective = bestRandomSolution.Indicator!.Score;

            if (bestRandomSolutionObjective > currentObjective ||
                _random.NextDouble() < Math.Exp((bestRandomSolutionObjective - currentObjective) / algorithmConfiguration.Temperature))
            {
                currentSolution = bestRandomSolution;
                currentObjective = bestRandomSolutionObjective;

                if (currentObjective > bestObjective)
                {
                    bestSolution = currentSolution;
                    bestObjective = currentObjective;
                }
            }
            algorithmConfiguration.Temperature *= algorithmConfiguration.CoolingRate;
        }
        return bestSolution;
    }

    private VoivodeshipState GetBestRandomSolution(List<VoivodeshipState> randomStates)
    {
        throw new NotImplementedException();
    }

    private List<VoivodeshipState> GenerateRandomSolutions(VoivodeshipState currentSolution, double stepsize, int neighboursAmount)
    {
        var solutions = new List<VoivodeshipState>();
        for (int i = 0; i < neighboursAmount; i++)
        {
            solutions.Add(GenerateSolution(currentSolution, stepsize));
        }
        return solutions;
    }

    private VoivodeshipState GenerateSolution(VoivodeshipState voivodeshipState, double stepsize)
    {
        var voivodeshipClone = _dbRepository.GetVoivodeShipClone(voivodeshipState.ActualConfiguration);
        return new VoivodeshipState(); //TBD
        
    }

    private InitialStateRequest GetInitialStateRequestFromOptimizeDistrictRequest(OptimizeLocalDistrictsRequest request)
    {
        return new InitialStateRequest()
        {
            VoivodeshipName = request.DistrictInformation.VoivodeshipName,
            Year = request.DistrictInformation.Year,
        };
    }
}
