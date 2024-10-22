using System;
using System.Collections.Generic;

namespace SimulatedAnnealing.Server.Models.Algorithm.Fixed;

public partial class Neighbor
{
    public int Id { get; set; }
    public int? CountyId { get; set; }
    public int? NeighborId { get; set; }
    public virtual County? County { get; set; }
}
