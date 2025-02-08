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
    public SimulatedAnnealingService(ComplianceService complianceService, IDbRepository dbRepository, VoivodeshipStateBuilder stateBuilder)
    {
        _complianceService = complianceService;
        _dbRepository = dbRepository;
        _stateBuilder = stateBuilder;
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


        return new VoivodeshipState();
        


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
