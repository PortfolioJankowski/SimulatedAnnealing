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

        public bool AreCountiesNeighbouring(int countyXId, int countyYId)
        {
            var countyX = _context.Powiaties
                .Include(p => p.Sasiedzis)
                .FirstOrDefault(p => p.PowiatId == countyXId);

            if (countyX == null || countyX.Sasiedzis == null) return false;

            return countyX.Sasiedzis.Any(p => p.SasiadId == countyYId);
        }

        public bool IsDistrictBoundaryUnbroken(int districtId)
        {
            // Fetch all counties in the district along with their neighbors (Sasiedzis)
            var counties = _context.Powiaties
                .Where(p => p.OkregId == districtId)
                .Include(p => p.Sasiedzis)  // Include neighboring counties
                .ToList();

            if (counties.Count == 0)
                return false; // No counties, boundary cannot be unbroken

            // Create a set to track visited counties
            var visitedCounties = new HashSet<int>();

            // Start DFS or BFS from the first county in the district
            DFS(counties[0].PowiatId, counties, visitedCounties);

            // Check if all counties were visited (i.e., connected)
            return visitedCounties.Count == counties.Count;
        }

        // Helper DFS method to traverse neighboring counties
        private void DFS(int countyId, List<Powiaty> counties, HashSet<int> visitedCounties)
        {
            if (visitedCounties.Contains(countyId))
                return;

            // Mark the county as visited
            visitedCounties.Add(countyId);

            // Get the current county from the list
            var currentCounty = counties.FirstOrDefault(p => p.PowiatId == countyId);
            if (currentCounty == null || currentCounty.Sasiedzis == null)
                return;

            // Traverse neighboring counties (Sasiedzis)
            foreach (var neighbor in currentCounty.Sasiedzis)
            {
                // Only continue DFS if the neighbor is within the same district
                if (counties.Any(c => c.PowiatId == neighbor.SasiadId))
                {
                    DFS((int)neighbor.SasiadId!, counties, visitedCounties);
                }
            }
        }


    }
}
