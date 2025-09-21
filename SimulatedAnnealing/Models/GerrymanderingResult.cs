namespace SimulatedAnnealing.Models;

public partial class GerrymanderingResult
{
    public int Id { get; set; }

    public string Configuration { get; set; } = null!;

    public int Iterations { get; set; }

    public string ChoosenParty { get; set; } = null!;

    public int? ElectoralYear { get; set; }

    public DateTime? CreatedAt { get; set; }

    public double PackingThreshold { get; set; }
    public double CrackingThreshold { get; set; }
    public double PackingWeight { get; set; }
    public double CrackingWeight { get; set; }
    public double InitialScore { get; set; }
    public double FinalScore { get; set; }
    public double ScoreChange { get; set; }
    public double InitialSeats { get; set; }
    public double FinalSeats { get; set; }
    public double SeatsChange { get; set; }
    public string Voivodeship { get; set; } = null!;
    public string Results { get; set; } = null!;
}
