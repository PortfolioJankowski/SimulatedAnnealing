using System;
using System.Collections.Generic;

namespace SimulatedAnnealing.Server.Models.Algorithm.Fixed.Parliament;

public partial class CountyPopulation
{
    public string CountyTeryt { get; set; } = null!;

    public int Year { get; set; }

    public int Population { get; set; }

    public virtual TerytCounty CountyTerytNavigation { get; set; } = null!;
}
