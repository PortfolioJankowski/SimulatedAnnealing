using SimulatedAnnealing.Server.Models.Algorithm.Fixed;
using SimulatedAnnealing.Server.Services.Behavioral;

namespace SimulatedAnnealing.Server.Models.Algorithm.Variable;

public class VoivodeshipState
{
    public Voivodeship ActualConfiguration { get; set; } = null!;
    public Indicator? Indicator { get; set; }
    public double PopulationIndex { get; set; }
    public int VoivodeshipSeatsAmount { get; set; }
    public int VoivodeshipInhabitants { get; set; }
    public Dictionary<District, Dictionary<string, int>>? DistrictVotingResults { get; set; }   
}

