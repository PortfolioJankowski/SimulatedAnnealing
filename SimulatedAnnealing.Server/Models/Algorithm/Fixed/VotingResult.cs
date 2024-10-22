using System;
using System.Collections.Generic;

namespace SimulatedAnnealing.Server.Models.Algorithm.Fixed;

public partial class VotingResult
{
    public int ResultsId { get; set; }
    public int Year { get; set; }
    public int? CountyId { get; set; }
    public string? Committee { get; set; }
    public int? NumberVotes { get; set; }
    public virtual County? County { get; set; }
}
