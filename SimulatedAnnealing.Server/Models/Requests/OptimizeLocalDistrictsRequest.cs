using SimulatedAnnealing.Server.Models.DTOs;

namespace SimulatedAnnealing.Server.Models.Requests
{
    public class OptimizeLocalDistrictsRequest
    {
        public LocalResultsRequest DistrictInformation { get; set; }
    }

    public class AlgorithmConfiguration
    {
        public double Temperature { get; set; }
        public double CoolingRate { get; set; }
        public double StepSize { get; set; }
        public long MaxIterations { get; set; }
        public double PackingThreshold { get; set;}
        public double CrackingThreshold { get; set; }
        public double PackingWeight { get; set; }   
        public double CrackingWeight { get;set; }
    }
}
