namespace SimulatedAnnealing.Server.Models.JNP
{
    public record JnpCheckResult
    {
        public bool IsValid => SeatsDeclared == SeatsCalculated;

        public int DistrictId { get; set; }
        public int SeatsDeclared { get; set; }
        public int SeatsCalculated { get; set; }
        public int Inhibitants { get; set; }
        public double DistrictJNP { get; set; } 
    }
}
