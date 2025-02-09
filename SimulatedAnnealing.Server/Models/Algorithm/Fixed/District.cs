using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimulatedAnnealing.Server.Models.Algorithm.Fixed;

public partial class District
{
    public int DistrictId { get; set; }
    public int Name { get; set; }
    public int? VoivodeshipsId { get; set; }
    public virtual ICollection<County> Counties { get; set; } = new List<County>();
    public virtual Voivodeship? Voivodeship { get; set; }
    [NotMapped]
    public int PartySeats { get; set; }
}
