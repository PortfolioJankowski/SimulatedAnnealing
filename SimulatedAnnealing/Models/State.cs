﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedAnnealing.Models
{
    public class State
    {
        public int ShiftNo {  get; set; }
        public Wojewodztwa ActualConfiguration { get; set; }
        public Indicator Indicator { get; set; }
       
        public Dictionary<string, int> VotingResults { get; set; }
    }
}