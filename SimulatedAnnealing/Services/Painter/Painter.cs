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

        public static void PrintOptimalState(State bestState, State initialState)
        {
            if (bestState.Indicator.Score == initialState.Indicator.Score)
            {
                Console.WriteLine($"NOTHING HAS CHANGED");
            }
            else
            {
                Console.WriteLine($"Optimal State => Change: {bestState.Indicator.Seats - initialState.Indicator.Seats} Seats: {bestState.Indicator.Seats}");

                var optimizedDistrictResults = bestState.DistrictVotingResults;

                // Iterate through each district in the optimized results
                foreach (var optimizedDistrict in optimizedDistrictResults)
                {
                    var districtKey = optimizedDistrict.Key; // Get the district key
                    var optimizedVotes = optimizedDistrict.Value; // Get optimized votes

                    // Retrieve initial votes for the same district
                    var initialVotesExists = initialState.DistrictVotingResults.TryGetValue(districtKey, out var initialDistrictVotes);

                    // Determine if there was a change in the district's votes
                    bool isChange = !initialVotesExists || !optimizedVotes.SequenceEqual(initialDistrictVotes);

                    // Print district information
                    Console.Write($"DISTRICT {districtKey.OkregId} ");

                    // Print powiaty list
                    Console.Write("      Powiaty: ");
                    Console.WriteLine(string.Join(", ", districtKey.Powiaties.Select(p => p.Nazwa))); // Assuming Powiaty has a property 'Nazwa'

                    Console.WriteLine($"      Seats: {districtKey.PartySeats} ");
                }
            }
            
        }


    }
}