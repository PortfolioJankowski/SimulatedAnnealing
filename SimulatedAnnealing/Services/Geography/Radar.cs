using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SimulatedAnnealing.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedAnnealing.Services.Geography
{
    public class Radar
    {
        private SimulatedAnnealingContext _context;

        public Radar(SimulatedAnnealingContext context)
        {
            this._context = context;
        }

        public bool AreCountiesNeighbouring(Powiaty county, int neighborId)
        {
            return county.PowiatySasiadujace.Any(p => p.PowiatId == neighborId);
        }

        public bool IsDistrictBoundaryUnbroken(int districtId)
        {
            // Fetch all counties in the district along with their neighboring counties (PowiatySasiadujace)
            var counties = _context.Powiaties
                .Where(p => p.OkregId == districtId)
                .Include(p => p.PowiatySasiadujace) // Include neighboring counties
                .ToList();

            if (counties.Count == 0)
                return false; // No counties, so the boundary cannot be unbroken

            // Create a set to track visited counties
            var visitedCounties = new HashSet<int>();

            // Start DFS or BFS from the first county in the district
            DFS(counties[0].PowiatId, counties, visitedCounties);

            // Check if all counties were visited (i.e., all are connected)
            return visitedCounties.Count == counties.Count;
        }

        // Helper DFS method to traverse neighboring counties
        private void DFS(int countyId, List<Powiaty> counties, HashSet<int> visitedCounties)
        {
            if (visitedCounties.Contains(countyId))
                return; // County already visited

            // Mark the county as visited
            visitedCounties.Add(countyId);

            // Get the current county from the list
            var currentCounty = counties.FirstOrDefault(p => p.PowiatId == countyId);
            if (currentCounty == null || currentCounty.PowiatySasiadujace == null)
                return; // Safety check in case the county or its neighbors are null

            // Traverse neighboring counties (PowiatySasiadujace) if they belong to the same district
            foreach (var neighbor in currentCounty.PowiatySasiadujace)
            {
                if (counties.Any(c => c.PowiatId == neighbor.PowiatId) && !visitedCounties.Contains(neighbor.PowiatId))
                {
                    DFS(neighbor.PowiatId, counties, visitedCounties); // Continue DFS for the neighbor
                }
            }
        }



    }
}
