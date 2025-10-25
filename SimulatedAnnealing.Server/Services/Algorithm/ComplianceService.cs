using SimulatedAnnealing.Server.Models.Algorithm.Fixed;
using SimulatedAnnealing.Server.Models.Algorithm.Fixed.Parliament;

namespace SimulatedAnnealing.Server.Services.Behavioral;
public class ComplianceService
{
    private readonly IConfiguration _configuration;
    public ComplianceService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public static Dictionary<int, int> ParliamentDistrictsSeats2023 { get; } = new Dictionary<int, int>
    {
        // Numer okregu, liczba mandatow
        { 1, 12 },
        { 2, 8 },
        { 3, 14 },
        { 4, 12 },
        { 5, 13 },
        { 6, 15 },
        { 7, 12 },
        { 8, 12 },
        { 9, 10 },
        { 10, 9 },
        { 11, 12 },
        { 12, 8 },
        { 13, 14 },
        { 14, 10 },
        { 15, 9 },
        { 16, 10 },
        { 17, 9 },
        { 18, 12 },
        { 19, 20 },
        { 20, 12 },
        { 21, 12 },
        { 22, 11 },
        { 23, 15 },
        { 24, 14 },
        { 25, 12 },
        { 26, 14 },
        { 27, 9 },
        { 28, 7 },
        { 29, 9 },
        { 30, 9 },
        { 31, 12 },
        { 32, 9 },
        { 33, 16 },
        { 34, 8 },
        { 35, 10 },
        { 36, 12 },
        { 37, 9 },
        { 38, 9 },
        { 39, 10 },
        { 40, 8 },
        { 41, 12 }
    };

    public int GetVoivodeshipInhabitants(ICollection<District> districts)
    {
        return districts
             .SelectMany(d => d.Counties)
             .Sum(p => p.Inahabitants);
    }

    public int GetVoivodeshipInhabitants(ICollection<ParliamentDistrict> districts, int year)
    {
        return districts
            .SelectMany(d => d.TerytCounties)
            .SelectMany(c => c.CountyPopulations)
            .Where(p => p.Year == year)
            .Sum(p => p.Population);
    }

    public double GetPopulationIndex(int voivodeshipInhabitants, int voivodeshipSeatsAmount) => voivodeshipInhabitants / voivodeshipSeatsAmount;

    public bool AreLegalRequirementsMet(Voivodeship configuration, double populationIndex, bool isParliament = false)
    {
        int[] districtSeats;

        if (isParliament)
        {
            districtSeats = CalculateParliamentDistrictSeats(configuration.ParliamentDistricts, populationIndex);
        }
        else
        {
            districtSeats = CalculateDistrictSeats(configuration.Districts, populationIndex);
        }

        int totalSeats = districtSeats.Sum();
        int defaultSeatsAmount = 0;

        if (isParliament)
        {
            foreach (var district in configuration.ParliamentDistricts)
            {
               if (ComplianceService.ParliamentDistrictsSeats2023.TryGetValue(district.Id, out int value))
                {
                    defaultSeatsAmount += value;
                }
            }
        }
        else
        {
            defaultSeatsAmount = _configuration.GetSection("DistrictsSeats").GetValue<int>(configuration.Name);
        }

        if (totalSeats != defaultSeatsAmount)
        {
            if (isParliament)
            {
                districtSeats = AdjustParliamentSeats(new AdjustDTO(districtSeats, totalSeats, defaultSeatsAmount, new List<District>(), configuration.ParliamentDistricts));
            }
            else
            {
                districtSeats = AdjustSeats(new AdjustDTO(districtSeats, totalSeats, defaultSeatsAmount, configuration.Districts));
            }
        }

        if (isParliament)
        {
            return districtSeats.All(seats => seats is >= 7 and <= 20);
        }
        else
        {
            return districtSeats.All(seats => seats is >= 5 and <= 15);
        }
    }

    internal Dictionary<ParliamentDistrict, Dictionary<string, int>> CalculateResultsForParliamentDistricts(Voivodeship configuration, int maxSeats, double populationIndex, string choosenParty)
    {
        var districtSeats = CalculateParliamentDistrictSeats(configuration.ParliamentDistricts, populationIndex);
        int totalSeats = districtSeats.Sum();
        if (totalSeats != maxSeats)
        {
            districtSeats = AdjustParliamentSeats(new AdjustDTO(districtSeats, totalSeats, maxSeats, configuration.Districts, configuration.ParliamentDistricts));
        }

        return GetResultsForDistricts(configuration.ParliamentDistricts, districtSeats, choosenParty);
    }

    internal Dictionary<District, Dictionary<string, int>> 
        CalculateResultsForDistricts(Voivodeship configuration, int maxSeats, double populationIndex, string choosenParty)
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

    private Dictionary<ParliamentDistrict, Dictionary<string, int>> GetResultsForDistricts(ICollection<ParliamentDistrict> districts, int[] districtSeats, string choosenParty)
    {
        var results = new Dictionary<ParliamentDistrict, Dictionary<string, int>>();
        var districtArray = districts.ToArray();

        foreach (var district in districts)
        {
            var votingResults = district.TerytCounties.SelectMany(p => p.ParliamentVotingResults)
                .GroupBy(w => w.Comitee)
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

    private int[] AdjustParliamentSeats(AdjustDTO data)
    {
        var populationIndexes = GetParliamentDistrictPopulationIndexes(data.ParliamentDistricts!, data.ActualDistribution);

        while (data.CountedSeats != data.MaxSeats)
        {
            int index = populationIndexes.ToList().IndexOf(data.CountedSeats > data.MaxSeats ? populationIndexes.Min() : populationIndexes.Max());
            data.ActualDistribution[index] += data.CountedSeats > data.MaxSeats ? -1 : 1;
            data.CountedSeats += data.CountedSeats > data.MaxSeats ? -1 : 1;
            populationIndexes = GetParliamentDistrictPopulationIndexes(data.ParliamentDistricts!, data.ActualDistribution);
        }

        return data.ActualDistribution;
    }

    private double[] GetDistrictPopulationIndexes(ICollection<District> districts, int[] seatDistribution)
    {
        return districts.Select((district, i) => (double)district.Counties.Sum(p => p.Inahabitants) / (seatDistribution[i] != 0 ? seatDistribution[i] : 1)).ToArray();
    }

    private double[] GetParliamentDistrictPopulationIndexes(ICollection<ParliamentDistrict> districts, int[] seatDistribution)
    {
        return districts.Select((district, i) => (double)district.TerytCounties.SelectMany(c => c.CountyPopulations).Sum(p => p.Population) / (seatDistribution[i] != 0 ? seatDistribution[i] : 1)).ToArray();
    }

    public int[] CalculateDistrictSeats(ICollection<District> districts, double populationIndex)
    {
        return districts.Select(d => (int)Math.Round(d.Counties.Sum(p => p.Inahabitants) / populationIndex)).ToArray();
    }

    public int[] CalculateParliamentDistrictSeats(ICollection<ParliamentDistrict> districts, double populationIndex)
    {
        //int[] seats = new int[districts.Count];
        //int i = 0;
        //foreach (var district in districts)
        //{
        //    if (ParliamentDistrictsSeats2023.TryGetValue(district.Id, out int value))
        //    {
        //        seats[i] = value;
        //    }
        //    i++;
        //}

        //return seats;
       return districts.Select(d => (int)Math.Round(d.TerytCounties.SelectMany(d=> d.CountyPopulations).Sum(p => p.Population) / populationIndex)).ToArray();
    }

    internal int CalculateSeatsAmountForVoievodianship(long inhabitants)
    {
        return inhabitants > 2_000_000 ? 30 + (int)Math.Ceiling((inhabitants - 2_000_000) / 500_000.0) * 3 : 30;
    }

    internal int CalculateSeatsAmountForVoievodianshipParliament(params int[] ids)
    {
        int sum = 0;
        foreach (int id in ids)
        {
            if (ParliamentDistrictsSeats2023.TryGetValue(id, out int value))
            {
                sum += value;
            }
        }
        return sum;
    }
}


public class AdjustDTO
{
    public int[] ActualDistribution { get; }
    public int CountedSeats { get; set; }
    public int MaxSeats { get; }
    public ICollection<District> Districts { get; }
    public ICollection<ParliamentDistrict>? ParliamentDistricts {get;}

    public AdjustDTO(int[] actualDistribution, int countedSeats, int maxSeats, ICollection<District> districts, ICollection<ParliamentDistrict>? parliamentDistricts = null)
    {
        ActualDistribution = actualDistribution;
        CountedSeats = countedSeats;
        MaxSeats = maxSeats;
        Districts = districts;
        ParliamentDistricts = parliamentDistricts;
    }
}