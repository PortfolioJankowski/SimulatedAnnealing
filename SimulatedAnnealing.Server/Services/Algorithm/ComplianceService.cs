using SimulatedAnnealing.Server.Models.Algorithm.Fixed;
using SimulatedAnnealing.Server.Models.Algorithm.Variable;

namespace SimulatedAnnealing.Server.Services.Behavioral;
public class ComplianceService
{
    private readonly IConfiguration _configuration;
    public ComplianceService(IConfiguration configuration)
    {

        _configuration = configuration;
    }

    public int GetVoivodeshipInhabitants(ICollection<District> districts)
    {
        return districts
             .SelectMany(d => d.Counties)
             .Sum(p => p.Inahabitants);
    }

    public double GetPopulationIndex(int voivodeshipInhabitants, int voivodeshipSeatsAmount) => voivodeshipInhabitants / voivodeshipSeatsAmount;

    public bool AreLegalRequirementsMet(Voivodeship configuration, double populationIndex)
    {
        var districtSeats = CalculateDistrictSeats(configuration.Districts, populationIndex);
        int totalSeats = districtSeats.Sum();
        var defaultSeatsAmount = _configuration.GetSection("DistrictsSeats").GetValue<int>(configuration.Name);

        if (totalSeats != defaultSeatsAmount)
        {
            districtSeats = AdjustSeats(new AdjustDTO(districtSeats, totalSeats, defaultSeatsAmount, configuration.Districts));
        }

        return districtSeats.All(seats => seats is >= 5 and <= 15);
    }

    internal Dictionary<District, Dictionary<string, int>> CalculateResultsForDistricts(Voivodeship configuration, int maxSeats, double populationIndex,string choosenParty)
    {
        var districtSeats = CalculateDistrictSeats(configuration.Districts, populationIndex);
        int totalSeats = districtSeats.Sum();

        if (totalSeats != maxSeats)
        {
            districtSeats = AdjustSeats(new AdjustDTO(districtSeats, totalSeats, maxSeats, configuration.Districts));
        }

        return GetResultsForDistricts(configuration.Districts, districtSeats, choosenParty);
    }

    private Dictionary<District, Dictionary<string, int>> GetResultsForDistricts(ICollection<District> districts, int[] districtSeats, string choosenParty)
    {
        var results = new Dictionary<District, Dictionary<string, int>>();
        var districtArray = districts.ToArray();

        foreach (var district in districts)
        {
            var votingResults = district.Counties.SelectMany(p => p.VotingResults)
                .GroupBy(w => w.Committee)
                .ToDictionary(g => g.Key, g => g.Sum(w => (int)w.NumberVotes!));

            int totalSeats = districtSeats[Array.IndexOf(districtArray, district)];
            var seatDistribution = DistributeSeats(votingResults, totalSeats);

            results[district] = seatDistribution;
            district.PartySeats = seatDistribution.GetValueOrDefault(choosenParty, 0);
        }

        return results;
    }

    private Dictionary<string, int> DistributeSeats(Dictionary<string, int> votes, int seats)
    {
        var quotients = votes.ToDictionary(v => v.Key, v => Enumerable.Range(1, seats).Select(i => (double)v.Value / i).ToList());
        var seatResults = new Dictionary<string, int>();

        for (int i = 0; i < seats; i++)
        {
            var (winner, maxQuotient) = quotients.SelectMany(q => q.Value.Select(v => (q.Key, v))).MaxBy(q => q.v);
            seatResults[winner] = seatResults.GetValueOrDefault(winner, 0) + 1;
            quotients[winner].Remove(maxQuotient);
        }

        return seatResults;
    }

    private int[] AdjustSeats(AdjustDTO data)
    {
        var populationIndexes = GetDistrictPopulationIndexes(data.Districts, data.ActualDistribution);

        while (data.CountedSeats != data.MaxSeats)
        {
            int index = populationIndexes.ToList().IndexOf(data.CountedSeats > data.MaxSeats ? populationIndexes.Min() : populationIndexes.Max());
            data.ActualDistribution[index] += data.CountedSeats > data.MaxSeats ? -1 : 1;
            data.CountedSeats += data.CountedSeats > data.MaxSeats ? -1 : 1;
            populationIndexes = GetDistrictPopulationIndexes(data.Districts, data.ActualDistribution);
        }

        return data.ActualDistribution;
    }

    private double[] GetDistrictPopulationIndexes(ICollection<District> districts, int[] seatDistribution)
    {
        return districts.Select((district, i) => (double)district.Counties.Sum(p => p.Inahabitants) / (seatDistribution[i] != 0 ? seatDistribution[i] : 1)).ToArray();
    }

    public int[] CalculateDistrictSeats(ICollection<District> districts, double populationIndex)
    {
        return districts.Select(d => (int)Math.Round(d.Counties.Sum(p => p.Inahabitants) / populationIndex)).ToArray();
    }

    internal int CalculateSeatsAmountForVoievodianship(long inhabitants)
    {
        return inhabitants > 2_000_000 ? 30 + (int)Math.Ceiling((inhabitants - 2_000_000) / 500_000.0) * 3 : 30;
    }

}

public class AdjustDTO
{
    public int[] ActualDistribution { get; }
    public int CountedSeats { get; set; }
    public int MaxSeats { get; }
    public ICollection<District> Districts { get; }

    public AdjustDTO(int[] actualDistribution, int countedSeats, int maxSeats, ICollection<District> districts)
    {
        ActualDistribution = actualDistribution;
        CountedSeats = countedSeats;
        MaxSeats = maxSeats;
        Districts = districts;
    }
}