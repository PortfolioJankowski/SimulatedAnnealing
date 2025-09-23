using System.ComponentModel.DataAnnotations.Schema;

namespace SimulatedAnnealing.Server.Models.Algorithm.Fixed.Parliament;

public partial class TerytCounty
{
    public string Teryt { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int? DistrictId { get; set; }

    public virtual ICollection<CountyPopulation> CountyPopulations { get; set; } = new List<CountyPopulation>();

    public virtual ParliamentDistrict? District { get; set; }

    public virtual ICollection<ParliamentVotingResult> ParliamentVotingResults { get; set; } = new List<ParliamentVotingResult>();

    public virtual ICollection<TerytNeighbor> TerytNeighborCountyTerytNavigations { get; set; } = new List<TerytNeighbor>();

    public virtual ICollection<TerytNeighbor> TerytNeighborNeighborTerytNavigations { get; set; } = new List<TerytNeighbor>();
    [NotMapped]
    public virtual List<TerytCounty> NeighboringCounties { get; set; } = new List<TerytCounty>();
}
