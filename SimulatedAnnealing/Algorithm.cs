using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedAnnealing
{
    public class Algorithm
    {
        static double ObjectiveFunction(double x)
        {
            return x * x; // Przykładowa funkcja: minimalizujemy x^2
        }

        // Generowanie sąsiedniego rozwiązania
        static double GenerateNeighbor(double current, double stepSize)
        {
            Random rand = new Random();
            return current + (rand.NextDouble() * 2 - 1) * stepSize; // Losowa zmiana
        }

        // Algorytm symulowanego wyżarzania
        public static double Optimize(double initialSolution, double initialTemperature, double coolingRate, double stepSize, int maxIterations)
        {
            double currentSolution = initialSolution;
            double currentObjective = ObjectiveFunction(currentSolution);
            double bestSolution = currentSolution;
            double bestObjective = currentObjective;

            double temperature = initialTemperature;

            Random rand = new Random();

            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                double neighbor = GenerateNeighbor(currentSolution, stepSize);
                double neighborObjective = ObjectiveFunction(neighbor);

                // Sprawdź, czy zaakceptować nowe rozwiązanie
                if (neighborObjective < currentObjective || rand.NextDouble() < Math.Exp((currentObjective - neighborObjective) / temperature))
                {
                    currentSolution = neighbor;
                    currentObjective = neighborObjective;

                    // Aktualizuj najlepsze rozwiązanie
                    if (currentObjective < bestObjective)
                    {
                        bestSolution = currentSolution;
                        bestObjective = currentObjective;
                    }
                }

                // Schładzanie
                temperature *= coolingRate;
            }

            return bestSolution;
        }
    }
}
