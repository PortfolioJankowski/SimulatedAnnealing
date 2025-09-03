using SimulatedAnnealing.Server.Models.DTOs;

namespace SimulatedAnnealing.Server.Models.Requests
{
    public class OptimizeLocalDistrictsRequest
    {
        public LocalResultsRequest DistrictInformation { get; set; }
    }
}
