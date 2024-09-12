using SimulatedAnnealing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedAnnealing.Services.Geography
{
    internal class Radar
    {
        private SimulatedAnnealingContext context;

        public Radar(SimulatedAnnealingContext context)
        {
            this.context = context;
        }

        public bool areCountiesNeighbouring()
        {
            return true;
        }

        public bool areDistrictsNeighbouring()
        {
            return true;
        }
    }
}
