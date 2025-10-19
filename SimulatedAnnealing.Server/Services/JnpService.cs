using SimulatedAnnealing.Server.Models.Algorithm.Variable;
using SimulatedAnnealing.Server.Models.DTOs;
using SimulatedAnnealing.Server.Models.JNP;
using SimulatedAnnealing.Server.Services.Algorithm;
using SimulatedAnnealing.Server.Services.Behavioral;
using SimulatedAnnealing.Server.Services.Database;

namespace SimulatedAnnealing.Server.Services
{
    public class JnpService(
        IConfiguration configuration,
        IDbRepository dbRepository,
        ParliamentSeatAllocationService allocationService,
        ComplianceService complianceService)
    {

        public async Task<CheckJnpResponse> CheckParliamentJnpAsync(int year)
        {
            var wojewodztwaNazwy = await dbRepository.GetVoivodeshipNamesAsync();

            var stany = new List<VoivodeshipState>();

            foreach (var nazwa in wojewodztwaNazwy)
            {
                var localResultsRequest = new LocalResultsRequest
                {
                    VoivodeshipName = nazwa,
                    Year = year
                };

                var stateBuilder = new VoivodeshipStateBuilder(complianceService, dbRepository);
                stateBuilder.SetParliament(true);

                var stan = stateBuilder
                    .SetVoivodeship(localResultsRequest).Result
                    .CalculateInhabitants()
                    .CalculateVoievodianshipSeatsAmount()
                    .CalculatePopulationIndex()
                    .CalculateDistrictResults("JNP")
                    .Build();

                stany.Add(stan);
            }

            var okregi = stany.SelectMany(s => s.ActualConfiguration.ParliamentDistricts).ToList();
            await allocationService.AllocateSeatsAsync(okregi);

            var results = new List<JnpCheckResult>();

            foreach (var okreg in okregi)
            {
                var seatsDeclared = ComplianceService.ParliamentDistrictsSeats2023
                    .First(d => d.Key == okreg.Id).Value;

                var seatsCalculated = okreg.SeatsToAllocate;

                var result = new JnpCheckResult
                {
                    DistrictId = okreg.Id,
                    SeatsCalculated = seatsCalculated,
                    SeatsDeclared = seatsDeclared,
                    Inhibitants = okreg.Population,
                    DistrictJNP = okreg.DistrictJNP,
                };

                results.Add(result);
            }

            return new CheckJnpResponse { Results = results, JNP = okregi.Sum(o => o.Population)/460 };
        }
    }
}

