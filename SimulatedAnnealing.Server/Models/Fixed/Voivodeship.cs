using System;
using System.Collections.Generic;

namespace SimulatedAnnealing.Server.Models.Fixed;

public partial class Voivodeship
{
    public int VoivodeshipsId { get; set; }
    public string Name { get; set; } = null!;
    public virtual ICollection<District> Districts { get; set; } = new List<District>();
}
