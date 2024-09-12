using System;
using System.Collections.Generic;
using System.Numerics;

namespace SimulatedAnnealing.Models;

public partial class Powiaty
{
    public int PowiatId { get; set; }

    public string Nazwa { get; set; } = null!;

    public int? OkregId { get; set; }

    public decimal? NajmniejszaSzerokoscGeograficzna { get; set; }

    public decimal? NajwiekszaSzerokoscGeograficzna { get; set; }

    public decimal? NajmniejszaWysokoscGeograficzna { get; set; }

    public decimal? NajwiekszaWysokoscGeograficzna { get; set; }

    public virtual Okregi? Okreg { get; set; }

    public int LiczbaMieszkancow { get; set; }

    public virtual ICollection<Wyniki> Wynikis { get; set; } = new List<Wyniki>();
}
