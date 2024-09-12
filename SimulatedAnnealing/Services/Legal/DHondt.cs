using SimulatedAnnealing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedAnnealing.Services.Legal
{
    public class DHondt
    {
        private readonly ElectoralCodex _codex;
        public DHondt(ElectoralCodex codex)
        {
            _codex = codex;
        }
        public Dictionary<string, int> CalculateVotingResults(State _state)
        {
            ICollection<Okregi> okregi = _state.ActualConfiguration.Okregis;
            long inhabitants = GetInhabitants(okregi);
            int availableSeats = _codex.CalculateSeatsAmountForVoievodianship(inhabitants);


            //TODO => dHondt algorithm implementation
            //int licznikMandatow = okregi.Sum(o => o.IloscMandatow);

            // Check if adjustment is needed
            //if (licznikMandatow != targetSeats)
            //{
            //    double minNorma, maxNorma;
            //    minNorma = double.MaxValue;
            //    maxNorma = double.MinValue;
            //    int nrOkregu;

            //    while (licznikMandatow != targetSeats)
            //    {
            //        var normyPrzedstawicielstwa = new Dictionary<double, int>();

            //        foreach (var okr in okregi)
            //        {
            //            okr.NormaPrzedstawicielska = (double)okr.Mieszkancy / okr.IloscMandatow;
            //            normyPrzedstawicielstwa[okr.NormaPrzedstawicielska] = okr.Numer;

            //            if (okr.NormaPrzedstawicielska < minNorma)
            //            {
            //                minNorma = okr.NormaPrzedstawicielska;
            //            }
            //            if (okr.NormaPrzedstawicielska > maxNorma)
            //            {
            //                maxNorma = okr.NormaPrzedstawicielska;
            //            }
            //        }

            //        if (licznikMandatow > targetSeats)
            //        {
            //            // Find the lowest representative norm
            //            nrOkregu = normyPrzedstawicielstwa[minNorma];
            //            var okr = okregi.First(o => o.Numer == nrOkregu);
            //            okr.IloscMandatow--;
            //            licznikMandatow--;
            //        }
            //        else
            //        {
            //            // Find the highest representative norm
            //            nrOkregu = normyPrzedstawicielstwa[maxNorma];
            //            var okr = okregi.First(o => o.Numer == nrOkregu);
            //            okr.IloscMandatow++;
            //            licznikMandatow++;
            //        }

            //        // Reset min and max norms for the next iteration
            //        minNorma = double.MaxValue;
            //        maxNorma = double.MinValue;
            //    }
            //}
            Dictionary<string, int> results = new Dictionary<string, int>();

            
            return new Dictionary<string, int>();
        }

        public long GetInhabitants(ICollection<Okregi> Okregi)
        {
            long inhabitants = 0;
            foreach (var okreg in Okregi)
            {
                foreach (var powiat in okreg.Powiaties)
                {
                    inhabitants += powiat.LiczbaMieszkancow;
                }
            }
            return inhabitants;
        }

        public Dictionary<string, int> CalculateSeats(Dictionary<string, int> results)
        {
            return new Dictionary<string, int>();
        }
    }
}
