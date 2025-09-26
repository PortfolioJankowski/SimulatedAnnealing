using SimulatedAnnealing.Server.Models.Algorithm.Fixed;
using SimulatedAnnealing.Server.Models.Algorithm.Fixed.Parliament;

namespace SimulatedAnnealing.Server.Services.Algorithm;
public class Geolocator
{
    public static bool IsCountyNeighbouringWithDistrict(County randomCounty, District district)
    {
        foreach (var county in district.Counties)
        {
            if (AreCountiesNeighbouring(randomCounty.CountyId, county))
            {
                return true;
            }
        }
        return false;
    }
 
    public static bool IsCountyNeighbouringWithDistrictParliament(TerytCounty randomCounty, ParliamentDistrict district)
    {
        foreach (var county in district.TerytCounties)
        {
            if (AreCountiesNeighbouring(randomCounty.Teryt, county))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsDistrictBoundariesUnbroken(District district)
    {
        if (district.Counties.Count == 0)
            return false;

        HashSet<int> visited = new HashSet<int>();
        Queue<County> countyQueue = new Queue<County>();
        HashSet<int> countyIds = district.Counties.Select(c => c.CountyId).ToHashSet();

        // BFS algorithm implementation
        var startCounty = district.Counties.First();
        countyQueue.Enqueue(startCounty);
        visited.Add(startCounty.CountyId);

        while (countyQueue.Count > 0)
        {
            var currentCounty = countyQueue.Dequeue();
            foreach (var neighbor in currentCounty.NeighboringCounties)
            {
                if (countyIds.Contains(neighbor.CountyId) && !visited.Contains(neighbor.CountyId))
                {
                    visited.Add(neighbor.CountyId);
                    countyQueue.Enqueue(district.Counties.Where(c => c.CountyId == neighbor.CountyId).First());
                }
            }
        }

        return visited.Count == district.Counties.Count;
    }

    public bool IsParliamentDistrictBoundariesUnbroken(ParliamentDistrict district)
    {
        if (district.TerytCounties.Count == 0)
            return false;

        HashSet<string> visited = new HashSet<string>();
        Queue<TerytCounty> countyQueue = new Queue<TerytCounty>();
        HashSet<string> countyIds = district.TerytCounties.Select(c => c.Teryt).ToHashSet();

        // BFS algorithm implementation
        var startCounty = district.TerytCounties.First();
        countyQueue.Enqueue(startCounty);
        visited.Add(startCounty.Teryt);

        while (countyQueue.Count > 0)
        {
            var currentCounty = countyQueue.Dequeue();
            foreach (var neighbor in currentCounty.NeighboringCounties)
            {
                if (countyIds.Contains(neighbor.Teryt) && !visited.Contains(neighbor.Teryt))
                {
                    visited.Add(neighbor.Teryt);
                    countyQueue.Enqueue(district.TerytCounties.Where(c => c.Teryt == neighbor.Teryt).First());
                }
            }
        }

        return visited.Count == district.TerytCounties.Count;
    }

    private static bool AreCountiesNeighbouring(int randomCountyId, County county)
    {
        return county.NeighboringCounties.Any(c => c.CountyId == randomCountyId);
    }

    static bool AreCountiesNeighbouring(string teryt, TerytCounty county)
    {
        return county.NeighboringCounties.Any(c => c.Teryt == teryt);
    }
}

