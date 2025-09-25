using SimulatedAnnealing.Server.Models.Algorithm.Variable;
using SimulatedAnnealing.Server.Models.Configuration;
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
    private bool _isParliament = false;

    public VoivodeshipStateBuilder(ComplianceService complianceService, IDbRepository dbRepository)
    {
        _complianceService = complianceService;
        _dbRepository = dbRepository; 
    }

    public void SetParliament(bool isParliament)
    {
        _isParliament = isParliament;
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

        if (_isParliament)
        {
            _voivodeshipState.ActualConfiguration = await _dbRepository.GetParliamentVoivodeshipAsync(initialStateRequest);
            return this;
        }
        
        _voivodeshipState.ActualConfiguration = await _dbRepository.GetVoivodeshipAsync(initialStateRequest);
        return this;
    }
    public VoivodeshipStateBuilder CalculateInhabitants()
    {
        if (_isParliament)
        {
            _voivodeshipState.VoivodeshipInhabitants = _complianceService.GetVoivodeshipInhabitants(_voivodeshipState.ActualConfiguration.ParliamentDistricts);
            return this;
        }
        _voivodeshipState.VoivodeshipInhabitants = _complianceService.GetVoivodeshipInhabitants(_voivodeshipState.ActualConfiguration.Districts);
        return this;
    }

    public VoivodeshipStateBuilder CalculateVoievodianshipSeatsAmount()
    {
        if (_isParliament)
        {
            _voivodeshipState.VoivodeshipSeatsAmount = _complianceService.CalculateSeatsAmountForVoievodianshipParliament(_voivodeshipState.ActualConfiguration.ParliamentDistricts.Select(d => d.Id).ToArray());
            return this;
        }
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
        if (_isParliament)
        {
            _voivodeshipState.ParliamentDistrictVotingResults = _complianceService.CalculateResultsForParliamentDistricts(_voivodeshipState.ActualConfiguration!, _voivodeshipState.VoivodeshipSeatsAmount, _voivodeshipState.PopulationIndex, choosenParty);
        }
        else
        {
            _voivodeshipState.DistrictVotingResults = _complianceService.CalculateResultsForDistricts(_voivodeshipState.ActualConfiguration!, _voivodeshipState.VoivodeshipSeatsAmount, _voivodeshipState.PopulationIndex, choosenParty);
        }
        return this;
    }

    internal VoivodeshipStateBuilder CalculateScore(OptimizeLocalDistrictsRequest request, AlgorithmConfiguration config)
    {
        if (_isParliament)
        {
            return this;
        }
        _voivodeshipState.Indicator = IndicatorService.SetNewIndicator(_voivodeshipState, request, config);
        return this;
    }
}

