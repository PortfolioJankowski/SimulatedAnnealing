using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimulatedAnnealing.Server.Models;

public partial class County
{
    public int CountyId { get; set; }

    public string Name { get; set; } = null!;

    public int? DistrictId { get; set; }

    public int Inahabitants { get; set; }

    public virtual District? District { get; set; }

    [NotMapped]
    public virtual List<County> NeighbouringCounties { get; set; } = new List<County>();

    public virtual ICollection<Neighbour> Neighbours { get; set; } = new List<Neighbour>();

    public virtual ICollection<VotingResult> VotingResults { get; set; } = new List<VotingResult>();
}
