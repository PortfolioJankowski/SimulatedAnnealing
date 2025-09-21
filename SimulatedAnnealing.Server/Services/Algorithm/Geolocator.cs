using SimulatedAnnealing.Server.Models.Algorithm.Fixed;

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
                //NEIGHBORING NIE MAJĄ NIC! TRZEBA W REPOZYTORIUM DODAĆ W NICH POWIATY ITD
                if (countyIds.Contains(neighbor.CountyId) && !visited.Contains(neighbor.CountyId))
                {
                    visited.Add(neighbor.CountyId);
                    countyQueue.Enqueue(district.Counties.Where(c => c.CountyId == neighbor.CountyId).First());
                }
            }
        }

        return visited.Count == district.Counties.Count;
    }

    private static bool AreCountiesNeighbouring(int randomCountyId, County county)
    {
        return county.NeighboringCounties.Any(c => c.CountyId == randomCountyId);
    }
}

