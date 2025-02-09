using Microsoft.AspNetCore.Identity;
using SimulatedAnnealing.Server.Models.Algorithm.Fixed;
using SimulatedAnnealing.Server.Models.Exceptions;

namespace SimulatedAnnealing.Server.Services.Algorithm;
public  class Geolocator
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
        Queue<County> queue = new Queue<County>();

        //BFS algorithm implementation
        var startCounty = district.Counties.First();
        queue.Enqueue(startCounty);
        visited.Add(startCounty.CountyId);
        while (queue.Count > 0)
        {
            var currentCounty = queue.Dequeue();
            foreach(var neighbor in currentCounty.NeighboringCounties)
            {
                if (district.Counties.Any(c => c.CountyId == neighbor.CountyId) && !visited.Contains(neighbor.CountyId))
                {
                    visited.Add(neighbor.CountyId);
                    queue.Enqueue(neighbor);
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

