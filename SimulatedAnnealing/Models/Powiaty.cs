﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimulatedAnnealing.Models;

public partial class Powiaty
{
    public int PowiatId { get; set; }

    public string Nazwa { get; set; } = null!;

    public int? OkregId { get; set; }

    public int LiczbaMieszkancow { get; set; }

    public virtual Okregi? Okreg { get; set; }

    public virtual ICollection<Sasiedzi> Sasiedzis { get; set; } = new List<Sasiedzi>();

    [NotMapped]
    public virtual List<Powiaty> PowiatySasiadujace { get; set; } = new List<Powiaty>();

    public virtual ICollection<Wyniki> Wynikis { get; set; } = new List<Wyniki>();
}
