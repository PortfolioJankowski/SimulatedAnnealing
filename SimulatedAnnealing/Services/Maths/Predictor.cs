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
        public Indicator SetNewIndicator(State state)
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

            double totalVotesForChosenParty = 0; // Suma głosów dla wybranej partii
            double totalVotes = 0; // Suma głosów we wszystkich okręgach 

            foreach (var district in state.ActualConfiguration.Okregis)
            {         
                foreach (var county in district.Powiaties)
                {
                    var countyResults = county.Wynikis.Where(w => w.Rok == 2024).ToList();

                    if (countyResults.Any())
                    {
                        int countyVotes = countyResults.Sum(r=> r.LiczbaGlosow ?? 0) ; // Liczba głosów w powiecie
                        var choosenPartyResult = countyResults.Where(r => r.Komitet == Configuration.ChoosenPoliticalGroup).FirstOrDefault();
                        if (choosenPartyResult != null)
                        {
                            totalVotesForChosenParty += choosenPartyResult.LiczbaGlosow ?? 0;
                        }
                        totalVotes += countyVotes;
                    }
                }

                if (totalVotes > 0)
                {
                    double targetPartyPercentage = totalVotesForChosenParty / totalVotes;

                    // efekt pakowania rośnie, kiedy pakowanie okręgu jest większe niż pakowanie tresholdu
                    packingEffect += System.Math.Max(0, targetPartyPercentage - packingThreshold);
                    crackingEffect += System.Math.Max(0, crackingThreshold - targetPartyPercentage);
                }
            }

            // Calculate total effect
            return (packingWeight * packingEffect) + (crackingWeight * crackingEffect);
        }


    }
}
