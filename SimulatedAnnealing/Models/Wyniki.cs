using System;
using System.Collections.Generic;

namespace SimulatedAnnealing.Models;

public partial class Wyniki
{
    public int WynikiId { get; set; }
    
    public int Rok { get; set; }

    public int? PowiatId { get; set; }

    public string Komitet { get; set; }

    public int LiczbaGlosow { get; set; }

    public virtual Powiaty? Powiat { get; set; }
}
