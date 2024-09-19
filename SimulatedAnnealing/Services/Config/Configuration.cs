using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedAnnealing.Services.Config
{
    public static class Configuration
    {
        public const double InitialSolution = 10;
        public const double InitialTemperature = 100;
        public const double CoolingRate = 0.99;
        public const double StepSize = 1.0;
        public const int MaxIterations = 1000;


        public const double PackingThreshold = 0.65; // Próg dla pakowania
        public const double CrackingThreshold = 0.45; // Próg dla rozpadu


        public const double PackingWeight = 1.0;
        public const double CrackingWeight = 1.0;


        public const int DefaultSeatsAmount = 10; // Domyślna liczba mandatów w okręgu
        public const double PopulationIndexMultiplier = 1.5;

        public const string ChoosenPoliticalGroup = "KOMITET WYBORCZY PRAWO I SPRAWIEDLIWOŚĆ";
        public const string ChoosenVoievodenship = "małopolskie";

    }
}
