namespace SimulatedAnnealing.Models;

public partial class Sasiedzi
{
    public int Id { get; set; }

    public int? PowiatId { get; set; }

    public int? SasiadId { get; set; }

    public virtual Powiaty? Powiat { get; set; }
}
