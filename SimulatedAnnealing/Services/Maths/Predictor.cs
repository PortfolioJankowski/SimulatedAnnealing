using SimulatedAnnealing.Models;
using SimulatedAnnealing.Services.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace SimulatedAnnealing.Services.Math
{
    public class Predictor
    {


        internal Indicator setNewIndicator(State state)
        {
            var sum = GetSeatsSum(state);
            Indicator indicator = new Indicator()
            {

                Seats = sum,
                Score = sum + GetGerrymanderingScore(state),
            };
            return indicator;
        }

        private int GetSeatsSum(State state)
        {
            return state.DistrictVotingResults.Values
                .SelectMany(district => district)
                .Where(wyniki => wyniki.Key == Configuration.ChoosenPoliticalGroup)
                .Sum(mandaty => mandaty.Value);
        }

        private double GetGerrymanderingScore(State state)
        {
            double packingEffect = 0;
            double crackingEffect = 0;

            
            double packingThreshold = Configuration.PackingThreshold;
            double crackingThreshold = Configuration.CrackingThreshold;
            double packingWeight = Configuration.PackingWeight;
            double crackingWeight = Configuration.CrackingWeight;

            foreach (var district in state.ActualConfiguration.Okregis)
            {
                if (state.DistrictVotingResults.TryGetValue(district, out var votingResults)) //results for district Dict<party:seats>
                {
                    if (votingResults.TryGetValue(Configuration.ChoosenPoliticalGroup, out int targetPartyVotes))
                    {
                        int totalVotes = votingResults.Values.Sum();

                        // Procent głosów na partię docelową
                        double targetPartyPercentage = (double)targetPartyVotes / totalVotes;

                        // Ocena efektu pakowania
                        packingEffect += System.Math.Max(0, targetPartyPercentage - packingThreshold);

                        // Ocena efektu rozpadu
                        crackingEffect += System.Math.Max(0, crackingThreshold - targetPartyPercentage);
                    }
                }
            }

            // Obliczenie łącznego efektu
            var TotalEffect = packingWeight * packingEffect + crackingWeight * crackingEffect;

            // Aktualizacja właściwości klasy Indicator
         
            return TotalEffect;
        }
    }
}
