namespace SimulatedAnnealing.Server.Models.Algorithm.Fixed.Parliament;

public partial class ParliamentDistrict
{
    public int Id { get; set; }

    public int Name { get; set; }

    public int? VoivodeshipId { get; set; }

    public virtual ICollection<TerytCounty> TerytCounties { get; set; } = new List<TerytCounty>();
}
