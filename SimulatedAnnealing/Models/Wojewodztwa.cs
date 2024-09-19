using SimulatedAnnealing.Services.Legal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimulatedAnnealing.Models;

public partial class Wojewodztwa
{
    public int WojewodztwoId { get; set; }

    public string Nazwa { get; set; } = null!;

    public virtual ICollection<Okregi> Okregis { get; set; } = new List<Okregi>();

}

