using System;
using System.Collections.Generic;

namespace SimulatedAnnealing.Server.Models.Algorithm.Fixed.Parliament;

public partial class ParliamentVotingResult
{
    public int ResultsId { get; set; }

    public int Year { get; set; }

    public string CountyTeryt { get; set; } = null!;

    public string Comitee { get; set; } = null!;

    public int NumberVotes { get; set; }

    public virtual TerytCounty CountyTerytNavigation { get; set; } = null!;
}
