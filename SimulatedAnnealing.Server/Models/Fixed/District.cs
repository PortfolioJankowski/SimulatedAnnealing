using System;
using System.Collections.Generic;

namespace SimulatedAnnealing.Server.Models.Fixed;

public partial class District
{
    public int DistrictId { get; set; }
    public int Name { get; set; }
    public int? VoivodeshipsId { get; set; }
    public virtual ICollection<County> Counties { get; set; } = new List<County>();
    public virtual Voivodeship? Voivodeships { get; set; }

    private bool IsDistrictBoundaryUnbroken(District district)
    {
        if (district == null || district.Counties == null || district.Counties.Count == 0)
        {
            return false;
        }
        // Using a HashSet to track visited counties
        HashSet<int> visited = new HashSet<int>();
        Queue<County> queue = new Queue<County>();

        // Start BFS from the first county
        var startCounty = district.Counties.First();
        queue.Enqueue(startCounty);
        visited.Add(startCounty.CountyId);

        while (queue.Count > 0)
        {
            var currentCounty = queue.Dequeue();

            foreach (var neighbor in currentCounty.NeighboringCounties)
            {
                if (district.Counties.Any(p => p.CountyId == neighbor.CountyId) && !visited.Contains(neighbor.CountyId))
                {
                    visited.Add(neighbor.CountyId);
                    queue.Enqueue(neighbor);
                }
            }
        }
        // If all counties are visited, the district is unbroken
        return visited.Count == district.Counties.Count;
    }
}
