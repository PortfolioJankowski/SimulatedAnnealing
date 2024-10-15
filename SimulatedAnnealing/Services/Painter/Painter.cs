using SimulatedAnnealing.Models;
using SimulatedAnnealing.Services.Config;
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
                 int i = 0; 
                foreach (var okreg in bestState.ActualConfiguration.Okregis)
                {
                    var counties = string.Join(",",okreg.Powiaties.Select(c => c.Nazwa));
                    var oldCounties = string.Join(",", initialState.ActualConfiguration.Okregis
                        .Where(o => o.OkregId == okreg.OkregId)
                        .SelectMany(o => o.Powiaties).Select(p => p.Nazwa));
                    var results = bestState.DistrictVotingResults.Where(o => o.Key == okreg).Select(o => o.Value.ContainsKey(Configuration.ChoosenPoliticalGroup));
                    var oldResult = initialState.DistrictVotingResults.Where(o => o.Key == okreg).Select(o => o.Value.ContainsKey(Configuration.ChoosenPoliticalGroup));
                    // Fetch the new and old voting results for each district
                    var newResults = bestState.DistrictVotingResults.ContainsKey(okreg)
                        ? bestState.DistrictVotingResults[okreg]
                        : new Dictionary<string, int>();

                    var oldResults = initialState.DistrictVotingResults.ContainsKey(okreg)
                        ? initialState.DistrictVotingResults[okreg]
                        : new Dictionary<string, int>();

                    Console.WriteLine($"DISTRICT {okreg.OkregId}"); 
                    Console.WriteLine($"New configuration {counties}");
                    Console.WriteLine($"Old configuration {oldCounties}");
                    // Print voting results comparison for each political group
                    Console.WriteLine($"Voting results for {Configuration.ChoosenPoliticalGroup}:");

                    if (newResults.ContainsKey(Configuration.ChoosenPoliticalGroup))
                    {
                        Console.WriteLine($"New results for {Configuration.ChoosenPoliticalGroup} => {newResults[Configuration.ChoosenPoliticalGroup]} votes");
                    }
                    else
                    {
                        Console.WriteLine($"No new results for {Configuration.ChoosenPoliticalGroup}");
                    }

                    if (oldResults.ContainsKey(Configuration.ChoosenPoliticalGroup))
                    {
                        Console.WriteLine($"Old results for {Configuration.ChoosenPoliticalGroup} => {oldResults[Configuration.ChoosenPoliticalGroup]} votes");
                    }
                    else
                    {
                        Console.WriteLine($"No old results for {Configuration.ChoosenPoliticalGroup}");
                    }

                    // Print all political groups and their results in both states for this district
                    Console.WriteLine("Comparison of votes for all political groups:");
                    var allGroups = new HashSet<string>(newResults.Keys.Union(oldResults.Keys));

                    foreach (var group in allGroups)
                    {
                        var newVotes = newResults.ContainsKey(group) ? newResults[group] : 0;
                        var oldVotes = oldResults.ContainsKey(group) ? oldResults[group] : 0;
                        Console.WriteLine($"{group}: New => {newVotes} votes, Old => {oldVotes} votes");
                    }

                    // Highlight the separation between districts
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.WriteLine("************************************************************");
                    Console.ResetColor();  // Reset the console color after the message
                }


            }
            
            
        }


    }
}