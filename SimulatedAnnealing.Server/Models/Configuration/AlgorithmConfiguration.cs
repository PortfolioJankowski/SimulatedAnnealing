namespace SimulatedAnnealing.Server.Models.Configuration
{
    public class AlgorithmConfiguration
    {
        public double Temperature { get; set; }
        public double CoolingRate { get; set; }
        public double StepSize { get; set; }
        public long MaxIterations { get; set; }
        public double PackingThreshold { get; set; }
        public double CrackingThreshold { get; set; }
        public double PackingWeight { get; set; }
        public double CrackingWeight { get; set; }
    }
}
