using System;
using System.Collections.Generic;

namespace SimulatedAnnealing.Server.Models;

public partial class Voivodship
{
    public int VoivodshipsId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<District> Districts { get; set; } = new List<District>();
}
