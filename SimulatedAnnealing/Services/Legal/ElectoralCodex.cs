using SimulatedAnnealing.Models;
using SimulatedAnnealing.Services.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedAnnealing.Services.Legal
{
    public class ElectoralCodex
    {
        public bool areLegalRequrementsMet(State state)
        {
            foreach (var okr in state.DistrictVotingResults.Values)
            {
                foreach (var result in okr.Values)
                {
                    if (result > 15 || result < 5)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal Dictionary<Okregi, Dictionary<string,int>> CalculateResultsForDistricts(Wojewodztwa actualConfiguration, int maxSeats, double populationIndex)
        {
            int[] districtsSeats = CalculateDistrictsSeats(actualConfiguration.Okregis, populationIndex);
            int totalCountedSeats = districtsSeats.Sum();
            if (totalCountedSeats != maxSeats)
            {
                AdjustDTO adjustDTO = new AdjustDTO()
                {
                    actualDistribution = districtsSeats,
                    countedSeats = totalCountedSeats,
                    maxSeats = maxSeats,
                    districts = actualConfiguration.Okregis,
                };
                districtsSeats = AdjustSeats(adjustDTO);
            }
            return GetResultsForDistricts(actualConfiguration.Okregis, districtsSeats);
        }

        private Dictionary<Okregi, Dictionary<string, int>> GetResultsForDistricts(ICollection<Okregi> okregis, int[] districtsSeats)
        {
            Dictionary<Okregi, Dictionary<string, int>> results = new();

            foreach (var district in okregis)
            {
                Dictionary<string, int> votingResult = new();
                Dictionary<string, int> seatsResult = new();

                foreach (var powiat in district.Powiaties)
                {
                    foreach (var wynik in powiat.Wynikis)
                    {
                        if (votingResult.ContainsKey(wynik.Komitet))
                        {
                            votingResult[wynik.Komitet] += wynik.LiczbaGlosow;
                        }
                        else
                        {
                            votingResult[wynik.Komitet] = wynik.LiczbaGlosow;
                        }
                    }
                }

                int totalSeats = districtsSeats[Array.IndexOf(okregis.ToArray(), district)];
                var quotients = new Dictionary<string, List<double>>();

                foreach (var komitet in votingResult.Keys)
                {
                    quotients[komitet] = new List<double>();
                    for (int i = 1; i <= totalSeats; i++)
                    {
                        quotients[komitet].Add((double)votingResult[komitet] / i);
                    }
                }

                for (int i = 0; i < totalSeats; i++)
                {
                    var maxQuotient = quotients.SelectMany(q => q.Value).Max();
                    var winningKomitet = quotients.First(q => q.Value.Contains(maxQuotient)).Key;

                    if (seatsResult.ContainsKey(winningKomitet))
                    {
                        seatsResult[winningKomitet]++;
                    }
                    else
                    {
                        seatsResult[winningKomitet] = 1;
                    }

                    quotients[winningKomitet].Remove(maxQuotient);
                }

                results[district] = seatsResult;
            }

            return results;



        }

        private int[] AdjustSeats(AdjustDTO data)
        {
            double[] populationIndexes = GetDistrictsPopulationIndexes(data.districts, data.actualDistribution);
            double minIndex = double.MaxValue;
            double maxIndex = double.MinValue;  

            while (data.countedSeats != data.maxSeats)
            {
                foreach (var index in populationIndexes)
                {
                    maxIndex = index > maxIndex ? index : maxIndex;
                    minIndex = index < minIndex ? index : minIndex;
                }

                if (data.countedSeats > data.maxSeats)
                {
                    int position = populationIndexes.ToList().IndexOf(populationIndexes.Min());
                    data.actualDistribution[position] = data.actualDistribution[position] - 1;
                    data.countedSeats--;
                    
                } else
                {
                    int position = populationIndexes.ToList().IndexOf(populationIndexes.Max());
                    data.actualDistribution[position] = data.actualDistribution[position] + 1;
                    data.countedSeats++;
                }
                populationIndexes = GetDistrictsPopulationIndexes(data.districts, data.actualDistribution);
            }
            return data.actualDistribution;
        }

        private double[] GetDistrictsPopulationIndexes(ICollection<Okregi> districts, int[] seatsDistribution)
        {
            var output = new List<double>();

            int i = 0;
            foreach (var district in districts)
            {
                var districtInhibtiants = district.Powiaties.Sum(x => x.LiczbaMieszkancow);
                output.Add(districtInhibtiants / seatsDistribution[i]);
                i++;
                
            }
           return output.ToArray();
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
    public class AdjustDTO
    {
        public int[] actualDistribution { get; set; }
        public int countedSeats { get; set; }   
        public int maxSeats { get; set; }
        public ICollection<Okregi> districts { get; set; }
    }
}
