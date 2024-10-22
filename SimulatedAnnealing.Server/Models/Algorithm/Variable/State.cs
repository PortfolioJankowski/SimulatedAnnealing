using SimulatedAnnealing.Server.Models.Algorithm.Fixed;
using SimulatedAnnealing.Server.Services.Behavioral;

namespace SimulatedAnnealing.Server.Models.Algorithm.Variable;

public class State
{
    private readonly ComplianceService _complianceService;
    public State(ComplianceService complianceService)
    {
        _complianceService = complianceService;
    }

    public Voivodeship ActualConfiguration { get; set; } = null!;
    public Indicator? Indicator { get; set; }
    public double PopulationIndex { get; set; }
    public int VoivodeshipSeatsAmount { get; set; }
    public int VoivodeshipInhabitants { get; set; }
    public Dictionary<District, Dictionary<string, int>>? DistrictVotingResults { get; set; }

    private void CalculateDetails()
    {
        CalculateInhabitants();
        CalculateVoievodianshipSeatsAmount();
        CalculatePopulationIndex();
        CalculateDistrictResults();
    }

    private void CalculateDistrictResults()
    {
        DistrictVotingResults = _complianceService.CalculateResultsForDistricts(ActualConfiguration!, VoivodeshipSeatsAmount, PopulationIndex);
    }

    private void CalculatePopulationIndex()
    {
        PopulationIndex = VoivodeshipInhabitants / VoivodeshipSeatsAmount;
    }

    private void CalculateVoievodianshipSeatsAmount()
    {
        VoivodeshipSeatsAmount = _complianceService.CalculateSeatsAmountForVoievodianship(VoivodeshipInhabitants);
    }

    private void CalculateInhabitants()
    {
        VoivodeshipInhabitants = ActualConfiguration!.Districts
            .SelectMany(d => d.Counties)
            .Sum(p => p.Inahabitants);
    }
}

