namespace SimulatedAnnealing.Server.Models.Algorithm.Fixed.Parliament;

public partial class TerytNeighbor
{
    public int Id { get; set; }

    public string CountyTeryt { get; set; } = null!;

    public string NeighborTeryt { get; set; } = null!;

    public virtual TerytCounty CountyTerytNavigation { get; set; } = null!;

    public virtual TerytCounty NeighborTerytNavigation { get; set; } = null!;
}
