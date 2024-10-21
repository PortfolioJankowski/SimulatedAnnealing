using System;
using System.Collections.Generic;

namespace SimulatedAnnealing.Server.Models;

public partial class District
{
    public int DistrictId { get; set; }

    public int Name { get; set; }

    public int? VoivodshipsId { get; set; }

    public virtual ICollection<County> Counties { get; set; } = new List<County>();

    public virtual Voivodship? Voivodships { get; set; }
}
