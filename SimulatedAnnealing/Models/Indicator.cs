using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedAnnealing.Models
{
    public class Indicator
    {
        public int Seats { get; set; }

        // Amount of votes above the seat
        public int Score { get; set; }
        public Okregi BestPotentialDistrict { get; set; }
        public Okregi WorstPotentialDistrict { get; set; }
    }
}
