using SimulatedAnnealing.Models;
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

        internal Dictionary<Okregi, Dictionary<string,int>> CalculateResultsForDistricts(Wojewodztwa actualConfiguration, int maxSeats, double populationIndex)
        {

            
            int[] districtsSeats = CalculateDistrictsSeats(actualConfiguration.Okregis, populationIndex);
            int totalCountedSeats = districtsSeats.Sum();
            if (totalCountedSeats != maxSeats)
            {
                districtsSeats = AdjustSeats(actualConfiguration.Okregis, totalCountedSeats, maxSeats);
            }
           

            // Apply d'Hondt method to calculate the seats allocation
            var finalResults = new Dictionary<Okregi, Dictionary<string, int>>();
            

            return finalResults;
        }

        private int[] AdjustSeats(ICollection<Okregi> okregis, int totalCountedSeats, int maxSeats)
        {
            //obliczenie jednolitych norm okręgowych
            //while totalCountedSeats <> maxSeats
            //sprawdzian czy mandat jest nadwyżkowy czy nie
            //odejmowanie albo dodawanie
            throw new NotImplementedException();
        }

        private int[] CalculateDistrictsSeats(ICollection<Okregi> okregis, double populationIndex)
        {
            int[] output = new int[okregis.Count];
            List<Okregi> okregiList = okregis.ToList();
            for (int i = 0; i < okregis.Count; i++)
            {
                var sum = okregiList[i].Powiaties.Sum(p => p.LiczbaMieszkancow);
                output[i] = (int)System.Math.Round(sum / populationIndex);
            }
            return output;
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
