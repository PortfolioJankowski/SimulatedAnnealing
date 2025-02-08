using SimulatedAnnealing.Server.Models.Algorithm.Fixed;
using SimulatedAnnealing.Server.Models.Algorithm.Variable;

namespace SimulatedAnnealing.Server.Services.Behavioral;
public class ComplianceService
{
    public Dictionary<District, Dictionary<string, int>>? CalculateResultsForDistricts(Voivodeship Voivodeship, int voivodeshipSeatsAmount, double populationIndex)
    {
        throw new NotImplementedException();
    }

    public int CalculateSeatsAmountForVoievodianship(int voivodeshipInhabitants)
    {
        throw new NotImplementedException();
    }

    public int GetVoivodeshipInhabitants(ICollection<District> districts)
    {
        return districts
             .SelectMany(d => d.Counties)
             .Sum(p => p.Inahabitants);
    }

    public double GetPopulationIndex(int voivodeshipInhabitants, int voivodeshipSeatsAmount) => voivodeshipInhabitants / voivodeshipSeatsAmount;
}
