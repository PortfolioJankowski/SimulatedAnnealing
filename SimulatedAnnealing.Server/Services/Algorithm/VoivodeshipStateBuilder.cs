using SimulatedAnnealing.Server.Models.Algorithm.Variable;
using SimulatedAnnealing.Server.Models.DTOs;
using SimulatedAnnealing.Server.Models.Requests;
using SimulatedAnnealing.Server.Services.Behavioral;
using SimulatedAnnealing.Server.Services.Database;

namespace SimulatedAnnealing.Server.Services.Algorithm;
public class VoivodeshipStateBuilder
{
    private readonly VoivodeshipState _voivodeshipState = new();
    private readonly ComplianceService _complianceService;
    private readonly IDbRepository _dbRepository;
  
    public VoivodeshipStateBuilder(ComplianceService complianceService, IDbRepository dbRepository)
    {
        _complianceService = complianceService;
        _dbRepository = dbRepository;
    }

    public VoivodeshipState Build()
    {
        return _voivodeshipState;
    }
    public async Task<VoivodeshipStateBuilder> SetVoivodeship(LocalResultsRequest districtRequest)
    {
        var initialStateRequest = new InitialStateRequest()
        {
            VoivodeshipName = districtRequest.VoivodeshipName,
            Year = districtRequest.Year,
        };
        _voivodeshipState.ActualConfiguration = await _dbRepository.GetVoivodeshipAsync(initialStateRequest);
        return this;
    }
    public VoivodeshipStateBuilder CalculateInhabitants()
    {
        _voivodeshipState.VoivodeshipInhabitants = _complianceService.GetVoivodeshipInhabitants(_voivodeshipState.ActualConfiguration.Districts);
        return this;
    }

    public VoivodeshipStateBuilder CalculateVoievodianshipSeatsAmount()
    {
        _voivodeshipState.VoivodeshipSeatsAmount = _complianceService.CalculateSeatsAmountForVoievodianship(_voivodeshipState.VoivodeshipInhabitants);
        return this;
    }

    public VoivodeshipStateBuilder CalculatePopulationIndex()
    {
        _voivodeshipState.PopulationIndex = _complianceService.GetPopulationIndex(_voivodeshipState.VoivodeshipInhabitants, _voivodeshipState.VoivodeshipSeatsAmount);
        return this;
    }

    public VoivodeshipStateBuilder CalculateDistrictResults(string choosenParty)
    {
        _voivodeshipState.DistrictVotingResults = _complianceService.CalculateResultsForDistricts(_voivodeshipState.ActualConfiguration!, _voivodeshipState.VoivodeshipSeatsAmount, _voivodeshipState.PopulationIndex, choosenParty);
        return this;
    }

    internal VoivodeshipStateBuilder CalculateScore(OptimizeLocalDistrictsRequest request)
    {
        _voivodeshipState.Indicator = IndicatorService.SetNewIndicator(_voivodeshipState, request);
        return this;
    }
}

