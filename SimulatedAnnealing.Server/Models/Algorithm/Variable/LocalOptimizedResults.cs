namespace SimulatedAnnealing.Server.Services.Behavioral;
public class LocalOptimizedResults
{
    public string VoivodeshipName { get; set; }
    public double StartScore { get; set; }
    public Dictionary<string, int> InitialResult { get; set; }
    public Dictionary<string, int> OptimizedResults { get; set; }
    public Dictionary<int, IEnumerable<string>> NewConfiguration { get; set; }
    public double OptimizedScore { get; set; }

}
