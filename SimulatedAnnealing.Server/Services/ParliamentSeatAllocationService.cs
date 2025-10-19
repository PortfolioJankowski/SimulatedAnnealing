using SimulatedAnnealing.Server.Models.Algorithm.Fixed.Parliament;
using SimulatedAnnealing.Server.Services.Database;

namespace SimulatedAnnealing.Server.Services
{
    public class ParliamentSeatAllocationService
    {
        private readonly IDbRepository _dbRepository;

        public ParliamentSeatAllocationService(IDbRepository dbRepository)
        {
            _dbRepository = dbRepository;
        }

        /// <summary>
        /// Oblicza liczbę mandatów w okręgach zgodnie z art. 202 Kodeksu wyborczego.
        /// Zwraca listę ParliamentDistrict z uzupełnionymi polami SeatsToAllocate i DistrictJNP.
        /// </summary>
        public async Task<List<ParliamentDistrict>> AllocateSeatsAsync(List<ParliamentDistrict> districts)
        {
            double totalPopulation = districts.Sum(d => d.Population);
            const int totalSeats = 460;

            // Norma krajowa (jednolita)
            double nationalNorm = totalPopulation / totalSeats;

            // Wylicz mandaty wg normy i zaokrąglenie wg §202 pkt 1
            foreach (var d in districts)
            {
                double theoreticalSeats = d.Population / nationalNorm;
                int seatsRounded = (int)Math.Floor(theoreticalSeats + 0.5); // >=0.5 w górę

                d.SeatsToAllocate = seatsRounded;
                d.DistrictJNP = d.Population / (double)d.SeatsToAllocate; // lokalna norma przedstawicielstwa
            }

            // Korekta jeśli suma != 460
            int sumSeats = districts.Sum(d => d.SeatsToAllocate);

            if (sumSeats > totalSeats)
            {
                int toRemove = sumSeats - totalSeats;

                // Odejmij mandaty z najmniejszym DistrictJNP (najwięcej mieszkańców / mandat)
                var ordered = districts
                    .OrderBy(d => d.DistrictJNP)
                    .ToList();

                for (int i = 0; i < toRemove; i++)
                {
                    ordered[i].SeatsToAllocate--;
                    ordered[i].DistrictJNP = ordered[i].Population / (double)ordered[i].SeatsToAllocate;
                }
            }
            else if (sumSeats < totalSeats)
            {
                int toAdd = totalSeats - sumSeats;

                // Dodaj mandaty tam, gdzie DistrictJNP największe (najmniej mieszkańców / mandat)
                var ordered = districts
                    .OrderByDescending(d => d.DistrictJNP)
                    .ToList();

                for (int i = 0; i < toAdd; i++)
                {
                    ordered[i].SeatsToAllocate++;
                    ordered[i].DistrictJNP = ordered[i].Population / (double)ordered[i].SeatsToAllocate;
                }
            }

            return districts;
        }
    }
}

