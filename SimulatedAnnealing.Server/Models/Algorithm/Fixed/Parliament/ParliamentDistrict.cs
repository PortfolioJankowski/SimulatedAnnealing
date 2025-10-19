using System.ComponentModel.DataAnnotations.Schema;

namespace SimulatedAnnealing.Server.Models.Algorithm.Fixed.Parliament;

public partial class ParliamentDistrict
{
    public int Id { get; set; }

    public int Name { get; set; }

    public int? VoivodeshipId { get; set; }

    public virtual ICollection<TerytCounty> TerytCounties { get; set; } = new List<TerytCounty>();

    [NotMapped]
    public int PartySeats { get; set; }

    [NotMapped]
    public int SeatsToAllocate { get; set; }

    [NotMapped]
    public int Population  => TerytCounties.SelectMany(c => c.CountyPopulations).Sum(c => c.Population);

    [NotMapped]
    public double DistrictJNP { get; set; }
}
