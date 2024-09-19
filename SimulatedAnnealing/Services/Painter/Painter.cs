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

        public static void ShowResults(State state)
        {
            Console.WriteLine("Optimized State Details:");
            Console.WriteLine($"Indicator Score: {state.Indicator.Score}");

            Console.WriteLine("District Voting Results:");
            foreach (var district in state.DistrictVotingResults)
            {
                Console.WriteLine($"District: {district.Key}");
                foreach (var politicalGroup in district.Value)
                {
                    Console.WriteLine($"  Political Group: {politicalGroup.Key}, Votes: {politicalGroup.Value}");
                }
            }
            Console.WriteLine($"Population Index: {state.PopulationIndex}");
            Console.WriteLine($"Voivodeship Seats Amount: {state.VoivodeshipSeatsAmount}");
            Console.WriteLine($"Voivodeship Inhabitants: {state.VoivodeshipInhabitants}");
        }
    }
}
