using System;
using System.Collections.Generic;

namespace SimulatedAnnealing.Server.Models;

public partial class Neighbour
{
    public int Id { get; set; }

    public int? CountyId { get; set; }

    public int? NeighbourId { get; set; }

    public virtual County? County { get; set; }
}
