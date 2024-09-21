using SimulatedAnnealing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedAnnealing.Services.Painter
{
    public static class Paint
    {
        public static void Start()
        {
            Console.WriteLine("******** GERRYMANDERRING STARTING.. **********");
        }

        public static void ShowResults(State initialState, State bestState)
        {
            Console.WriteLine("*******************************************************");
            Console.WriteLine("Comparison of Initial State and Optimized State:");
            Console.WriteLine($"Initial Indicator Score: {initialState.Indicator.Score}");
            Console.WriteLine($"Optimized Indicator Score: {bestState.Indicator.Score}");
            Console.WriteLine("-------------------------------------------------------");

            var initialDistrictResults = initialState.DistrictVotingResults;
            var optimizedDistrictResults = bestState.DistrictVotingResults;

            // Compare the voting results for each district
            foreach (var district in optimizedDistrictResults)
            {
                var initialVotes = initialDistrictResults.ContainsKey(district.Key) ? initialDistrictResults[district.Key] : new Dictionary<string, int>();
                var optimizedVotes = district.Value;

                Console.WriteLine($"District: {district.Key.Numer}");
                foreach (var politicalGroup in optimizedVotes)
                {
                    var initialVoteCount = initialVotes.ContainsKey(politicalGroup.Key) ? initialVotes[politicalGroup.Key] : 0;
                    var optimizedVoteCount = politicalGroup.Value;
                    var voteChange = optimizedVoteCount - initialVoteCount;

                    // Display vote changes
                    Console.WriteLine($"  Political Group: {politicalGroup.Key}, Initial Votes: {initialVoteCount}, Optimized Votes: {optimizedVoteCount}, Change: {voteChange}");
                }

                // Display additional details for the district
                Console.WriteLine("  Detailed District Changes:");
                DisplayDistrictDetails(initialState, bestState, district.Key);
                Console.WriteLine("-------------------------------------------------------");
            }

            // Summary of population and seats
            DisplayPopulationAndSeatsComparison(initialState, bestState);

            Console.WriteLine("*******************************************************");
        }

        private static void DisplayDistrictDetails(State initialState, State bestState, Okregi district)
        {
            var initialDistrict = initialState.ActualConfiguration.Okregis.FirstOrDefault(o => o.OkregId == district.OkregId);
            var optimizedDistrict = bestState.ActualConfiguration.Okregis.FirstOrDefault(o => o.OkregId == district.OkregId);

            if (initialDistrict != null && optimizedDistrict != null)
            {
                Console.WriteLine($"    Optimized District Counties: {string.Join(", ", optimizedDistrict.Powiaties.Select(p => p.Nazwa))}");
            }
            else
            {
                Console.WriteLine("    District data is missing in one of the states.");
            }
        }

        private static void DisplayPopulationAndSeatsComparison(State initialState, State bestState)
        {
            
            Console.WriteLine($"Optimized Population Index: {bestState.PopulationIndex}");
            Console.WriteLine("-------------------------------------------------------");
            
            Console.WriteLine($"Optimized Voivodeship Seats Amount: {bestState.VoivodeshipSeatsAmount}");
            Console.WriteLine("-------------------------------------------------------");
           
            Console.WriteLine($"Optimized Voivodeship Inhabitants: {bestState.VoivodeshipInhabitants}");
            Console.WriteLine("-------------------------------------------------------");
        }

    }
}