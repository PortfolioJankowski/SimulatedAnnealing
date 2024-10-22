using SimulatedAnnealing.Server.Models.Fixed;
using SimulatedAnnealing.Server.Services.Behavioral;

namespace SimulatedAnnealing.Server.Models.Variable;

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
        this.CalculateInhabitants();
        this.CalculateVoievodianshipSeatsAmount();
        this.CalculatePopulationIndex();
        this.CalculateDistrictResults();
    }

    private void CalculateDistrictResults()
    {
        this.DistrictVotingResults = _complianceService.CalculateResultsForDistricts(this.ActualConfiguration!, this.VoivodeshipSeatsAmount, this.PopulationIndex);
    }

    private void CalculatePopulationIndex()
    {
        this.PopulationIndex = this.VoivodeshipInhabitants / this.VoivodeshipSeatsAmount;
    }

    private void CalculateVoievodianshipSeatsAmount()
    {
        this.VoivodeshipSeatsAmount = _complianceService.CalculateSeatsAmountForVoievodianship(this.VoivodeshipInhabitants);
    }

    private void CalculateInhabitants()
    {
        this.VoivodeshipInhabitants = this.ActualConfiguration!.Districts
            .SelectMany(d => d.Counties)
            .Sum(p => p.Inahabitants);
    }
}

