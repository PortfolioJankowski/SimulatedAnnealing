using SimulatedAnnealing.Models;
using SimulatedAnnealing.Services.Config;
using SimulatedAnnealing.Services.Geography;

public class Algorithm
{
    // Objective Function: Evaluate the state based on its Indicator.Score
    static double ObjectiveFunction(State state)
    {
        return state.Indicator.Score;
    }

    // Generate a neighboring state by moving a random county between districts
    static State GenerateNeighbor(State currentState, double stepSize)
    {
        var rand = new Random();
        var neighborState = new State
        {
            // Copy properties from the current state
            ActualConfiguration = currentState.ActualConfiguration,
            Indicator = currentState.Indicator,
            PopulationIndex = currentState.PopulationIndex,
            VoivodeshipSeatsAmount = currentState.VoivodeshipSeatsAmount,
            VoivodeshipInhabitants = currentState.VoivodeshipInhabitants,
            DistrictVotingResults = new Dictionary<Okregi, Dictionary<string, int>>(currentState.DistrictVotingResults)
        };

        // Get a random district and county
        var districts = neighborState.DistrictVotingResults.Keys.ToList();
        if (districts.Count == 0) return neighborState; // No districts to modify

        var selectedDistrict = districts[rand.Next(districts.Count)];
        var counties = neighborState.DistrictVotingResults[selectedDistrict].Keys.ToList();
        if (counties.Count == 0) return neighborState; // No counties to modify

        var selectedCounty = counties[rand.Next(counties.Count)];
        // Implement logic to move a county to another district
        MoveCountyToAnotherDistrict(neighborState, selectedCounty, selectedDistrict, rand);

        // Ensure boundary conditions and legality
        if (!AreDistrictBoundariesValid(neighborState))
        {
            return currentState; // If invalid, return the current state
        }

        return neighborState;
    }

    private static void MoveCountyToAnotherDistrict(State state, string county, Okregi currentDistrict, Random rand)
    {
        var districts = state.DistrictVotingResults.Keys.ToList();
        var newDistrict = districts.FirstOrDefault(d => !d.Equals(currentDistrict));
        if (newDistrict != null)
        {
            // Remove county from current district
            state.DistrictVotingResults[currentDistrict].Remove(county);

            // Add county to new district
            if (!state.DistrictVotingResults.ContainsKey(newDistrict))
            {
                state.DistrictVotingResults[newDistrict] = new Dictionary<string, int>();
            }
            state.DistrictVotingResults[newDistrict][county] = 1; // Example: add county with placeholder value
        }
    }

    private static bool AreDistrictBoundariesValid(State state)
    {
        SimulatedAnnealingContext context = new();
        Radar radar = new(context);
        List<Powiaty> powiaty = state.ActualConfiguration.Okregis
            .SelectMany(p => p.Powiaties)
            .ToList();

        foreach (var okr in state.ActualConfiguration.Okregis)
        {
            var powiatList = okr.Powiaties;

            foreach (var pow in powiatList)
            {
                foreach (var otherPow in powiatList)
                {
                    if (pow.PowiatId != otherPow.PowiatId)
                    {
                        if (!radar.areCountiesNeighbouring(pow.PowiatId, otherPow.PowiatId))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    // Simulated Annealing Optimization
    public static State Optimize(State initialState)
    {
        State currentSolution = initialState;
        double currentObjective = ObjectiveFunction(currentSolution);
        State bestSolution = currentSolution;
        double bestObjective = currentObjective;

        double temperature = Configuration.InitialTemperature;
        double coolingRate = Configuration.CoolingRate;
        double stepSize = Configuration.StepSize;
        int maxIterations = Configuration.MaxIterations;

        Random rand = new Random();

        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            State neighbor = GenerateNeighbor(currentSolution, stepSize);
            double neighborObjective = ObjectiveFunction(neighbor);

            // Check if we should accept the new solution
            if (neighborObjective < currentObjective || rand.NextDouble() < Math.Exp((currentObjective - neighborObjective) / temperature))
            {
                currentSolution = neighbor;
                currentObjective = neighborObjective;

                // Update best solution found
                if (currentObjective < bestObjective)
                {
                    bestSolution = currentSolution;
                    bestObjective = currentObjective;
                }
            }

            // Cooling
            temperature *= coolingRate;
        }

        return bestSolution;
    }
}