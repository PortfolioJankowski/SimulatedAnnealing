namespace SimulatedAnnealing.Server.Models.JNP;

public record CheckJnpResponse
{
    public List<JnpCheckResult> Results { get; set; }
    public int TotalSeatsCalculated => Results.Sum(r => r.SeatsCalculated);
    public int TotalSeatsDeclared => Results.Sum(r => r.SeatsDeclared);
    public double JNP { get; set; }
}
