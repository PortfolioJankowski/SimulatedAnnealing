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
    public double DistrictJNP { get; set; }

    public int GetPopulationForYear(int year)
    {
        return TerytCounties
            .SelectMany(c => c.CountyPopulations)
            .Where(cp => cp.Year == year)
            .Sum(cp => cp.Population);
    }
}
