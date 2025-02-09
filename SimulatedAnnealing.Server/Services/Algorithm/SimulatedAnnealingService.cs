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
    private readonly Geolocator _geolocator;
    
    public SimulatedAnnealingService(ComplianceService complianceService, IDbRepository dbRepository, VoivodeshipStateBuilder stateBuilder)
    {
        _complianceService = complianceService;
        _dbRepository = dbRepository;
        _stateBuilder = stateBuilder;
        _random = new Random();
        _geolocator = new Geolocator();
    }
    public async Task<VoivodeshipState> Optimize(OptimizeLocalDistrictsRequest request)
    {
        var currentSolution = _stateBuilder
            .SetVoivodeship(request.DistrictInformation).Result
            .CalculateInhabitants()
            .CalculateVoievodianshipSeatsAmount()
            .CalculatePopulationIndex()
            .CalculateDistrictResults(request.DistrictInformation.PoliticalParty)
            .Build();
            
        var algorithmConfiguration = request.AlgorithmConfiguration;
        var bestSolution = currentSolution;
        var bestObjective = currentSolution.Indicator!.Score;

        int neighboursAmount = 5;
        var currentObjective = currentSolution.Indicator!.Score;
        var bestRandomSolutionObjective = 0d;

        for (int i = 0; i < algorithmConfiguration.MaxIterations; i++)
        {
            var randomStates = await GenerateRandomSolutions(currentSolution, algorithmConfiguration.StepSize, neighboursAmount);
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

    private async Task<List<VoivodeshipState>> GenerateRandomSolutions(VoivodeshipState currentSolution, double stepsize, int neighboursAmount)
    {
        var solutions = new List<VoivodeshipState>();
        for (int i = 0; i < neighboursAmount; i++)
        {
            solutions.Add(await GenerateSolution(currentSolution, stepsize));
        }
        return solutions;
    }

    private async Task<VoivodeshipState> GenerateSolution(VoivodeshipState voivodeshipState, double stepsize)
    {
        var voivodeshipClone = await _dbRepository.GetVoivodeShipClone(voivodeshipState.ActualConfiguration);
        voivodeshipClone = await MoveRandomCounty(voivodeshipClone, voivodeshipState.PopulationIndex);

        if (!_complianceService.AreLegalRequirementsMet(voivodeshipClone, voivodeshipState.PopulationIndex))
        {
            return voivodeshipState; //Nothing changed
        }
        return GetVoivodeshipStateClone(voivodeshipState, voivodeshipClone);
    }

    private static VoivodeshipState GetVoivodeshipStateClone(VoivodeshipState voivodeshipState, Voivodeship voivodeshipClone)
    {
        return  new VoivodeshipState
        {
            ActualConfiguration = voivodeshipClone,
            PopulationIndex = voivodeshipState.PopulationIndex,
            VoivodeshipInhabitants = voivodeshipState.VoivodeshipInhabitants,
            VoivodeshipSeatsAmount = voivodeshipState.VoivodeshipSeatsAmount,
        };
    }

    private async Task<Voivodeship> MoveRandomCounty(Voivodeship voivodeshipClone, double populationIndex)
    {
        var randomDistrict = voivodeshipClone.Districts.OrderBy(d => _random.Next()).First();
        var randomCounty = randomDistrict.Counties.OrderBy(c => _random.Next()).First();
        District neighboringDistrict = null;
        int maxCountyShiftsAmount = 100;
        int attempt = 0;

        while (neighboringDistrict is null && attempt < maxCountyShiftsAmount)
        {
            attempt++;
            neighboringDistrict = voivodeshipClone.Districts
                                        .Where(d => d.DistrictId != randomDistrict.DistrictId)
                                        .OrderBy(d => _random.Next())
                                        .FirstOrDefault()!;

            if (neighboringDistrict != null && Geolocator.IsCountyNeighbouringWithDistrict(randomCounty, neighboringDistrict))
            {
                break;
            } 
            else
            {
                neighboringDistrict = null;
            }
        }
        if (neighboringDistrict is null)
            return voivodeshipClone; //No valid neighboring district found

        randomDistrict.Counties.Remove(randomCounty);
        neighboringDistrict.Counties.Add(randomCounty);

        if (AreDistrictsValid(voivodeshipClone, neighboringDistrict, randomDistrict, populationIndex))
            return voivodeshipClone;

        return RestoreOriginalConfiguration(voivodeshipClone, randomDistrict, neighboringDistrict, randomCounty);
    }

    private bool AreDistrictsValid(Voivodeship voivodeship, District neighboringDistrict, District randomDistrict, double populationIndex)
    {
        if (_geolocator.IsDistrictBoundariesUnbroken(randomDistrict) && _geolocator.IsDistrictBoundariesUnbroken(neighboringDistrict))
        {
            foreach (var dist in voivodeship.Districts)
            {
                if (_geolocator.IsDistrictBoundariesUnbroken(dist) && _complianceService.AreLegalRequirementsMet(voivodeship, populationIndex))
                {
                    return true; //New configuration is Valid!
                }
            }
            return false;
        }
        return false;
    }

    private Voivodeship RestoreOriginalConfiguration(Voivodeship config, District randomDistrict, District neighboringDistrict, County randomCounty)
    {
        randomDistrict.Counties.Add(randomCounty);
        neighboringDistrict.Counties.Remove(randomCounty);
        return config;
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
