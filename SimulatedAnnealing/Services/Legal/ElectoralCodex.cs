using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedAnnealing.Services.Legal
{
    public class ElectoralCodex
    {
        public bool areLegalRequrementsMet()
        {
            return true;
        }

        internal int CalculateSeatsAmountForVoievodianship(long inhabitants)
        {
            int seatsAmount;
            if (inhabitants > 2000000) 
            {
                double temp = System.Math.Ceiling((double)(inhabitants - 2000000)/500000);
                return 30 + (int)(temp * 3);
                
            } else
            {
                return 30;
            }
        }
    }
}
