using SimulatedAnnealing.Models;

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

        public bool IsCountyNeighbouringWithDistrict(Powiaty county, Okregi district)
        {
            foreach (var powiat in district.Powiaties)
            {
                if (AreCountiesNeighbouring(county, powiat.PowiatId))
                {
                    return true;
                }
            }
            return false;
        }


        public bool IsDistrictBoundaryUnbroken(Okregi district)
        {
            if (district == null || district.Powiaties == null || district.Powiaties.Count == 0)
            {
                return false;
            }

            // Using a HashSet to track visited counties
            HashSet<int> visited = new HashSet<int>();
            Queue<Powiaty> queue = new Queue<Powiaty>();

            // Start BFS from the first county
            var startCounty = district.Powiaties.First();
            queue.Enqueue(startCounty);
            visited.Add(startCounty.PowiatId);

            while (queue.Count > 0)
            {
                var currentCounty = queue.Dequeue();

                foreach (var neighbor in currentCounty.PowiatySasiadujace)
                {
                    if (district.Powiaties.Any(p => p.PowiatId == neighbor.PowiatId) && !visited.Contains(neighbor.PowiatId))
                    {
                        visited.Add(neighbor.PowiatId);
                        queue.Enqueue(neighbor);
                    }
                }
            }

            // If all counties are visited, the district is unbroken
            return visited.Count == district.Powiaties.Count;
        }



    }
}
