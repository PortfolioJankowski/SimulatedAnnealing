using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SimulatedAnnealing.Services.Config
{
    public static class Configuration
    {
        public static IConfigurationRoot Config { get; private set; }
        //static construction is used on first class use
        static Configuration()
        {
            try
            {
                Config = new ConfigurationBuilder()
                   
                    .SetBasePath (Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                    .Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading configuration: {ex.Message}");
                throw;
            }
        }
        public const double InitialSolution = 10;
        public const double InitialTemperature = 1000;
        public const double CoolingRate = 0.99;
        public const double StepSize = 1.0;
        public const int MaxIterations = 1000;


        public const double PackingThreshold = 0.439; // Próg dla pakowania
        public const double CrackingThreshold = 0.01; // Próg dla rozpadu


        public const double PackingWeight = 1.0;
        public const double CrackingWeight = 0.01;


        public const int DefaultSeatsAmount = 39; // Domyślna liczba mandatów w okręgu
        public const double PopulationIndexMultiplier = 1.5;

        public const string ChoosenPoliticalGroup = "KOMITET WYBORCZY PRAWO I SPRAWIEDLIWOŚĆ";
        public const string ChoosenVoievodenship = "małopolskie";
        public const int ElectoralYear = 2024;


           
    }
}
