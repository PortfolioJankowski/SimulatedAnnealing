using System;
using System.Collections.Generic;

namespace SimulatedAnnealing.Models;

public partial class Okregi
{
    public int OkregId { get; set; }

    public int Numer { get; set; }

    public int? WojewodztwoId { get; set; }

    public virtual ICollection<Powiaty> Powiaties { get; set; } = new List<Powiaty>();

    public virtual Wojewodztwa? Wojewodztwo { get; set; }
}
