using SimulatedAnnealing.Models;
using SimulatedAnnealing.Services.Config;
using SimulatedAnnealing.Services.Database;
using SimulatedAnnealing.Services.Geography;
using SimulatedAnnealing.Services.Legal;
using SimulatedAnnealing.Services.Math;
using System.Diagnostics;
using System.Numerics;

public class Algorithm
{
    private readonly Radar _radar;
    private readonly Predictor _predictor;
    private readonly ElectoralCodex _codex;
    private readonly DbRepository _dbRepository;


    public Algorithm(Radar radar, Predictor predictor, ElectoralCodex electoralCodex, DbRepository repository)
    {
        _radar = radar;
        _predictor = predictor;
        _codex = electoralCodex;
        _dbRepository = repository;
    }

    public double EvaluateState(State state)
    {
        if (state == null) throw new ArgumentNullException(nameof(state));
        return state.Indicator!.Score;
    }

    public Dictionary<int, List<int>> GetCurrentStateConfiguration(ICollection<Okregi> okregis){
        Dictionary<int, List<int>> output = new();
            foreach (var okr in okregis){
                List<int> countiesNumbers = okr.Powiaties.Select(p => p.PowiatId).ToList();
                output.Add(okr.OkregId, countiesNumbers);
        }
        return output;
    }

    public State CloneState(State currentState){
        State neighborState = new State();
        Dictionary<int, List<int>> neighborConfigurationSettings = GetCurrentStateConfiguration(currentState.ActualConfiguration!.Okregis);
        neighborState.ActualConfiguration = _dbRepository.GetVoiewodeshipClone(neighborConfigurationSettings);
        neighborState.CalculateDetails();
        neighborState.Indicator = _predictor.SetNewIndicator(neighborState);
        return neighborState;
    }

    public State GenerateNeighbor(State currentState, double stepSize)
    {
        if (currentState == null) throw new ArgumentNullException(nameof(currentState));

        // Create a deep copy of the current state
        var neighborState = CloneState(currentState);
        var random = new Random();
        neighborState.ActualConfiguration = MoveCounty(neighborState.ActualConfiguration!, random, neighborState.PopulationIndex);
      
        if (!_codex.AreLegalRequirementsMet(neighborState.ActualConfiguration, neighborState.PopulationIndex))
        {
            return currentState; 
        }
        return neighborState;
    }

    public Wojewodztwa MoveCounty(Wojewodztwa neighborConfiguration, Random random, double populationIndex)
    {
        Okregi randomDistrict = neighborConfiguration.Okregis.OrderBy(o => random.Next()).First();
        var randomCounty = randomDistrict.Powiaties.OrderBy(p => random.Next()).First();
        Okregi? neighboringDistrict = null;
        int maxAttempts = 100;
        int attempts = 0;

        while (neighboringDistrict == null && attempts < maxAttempts)
        {
            attempts++;
            neighboringDistrict = neighborConfiguration.Okregis
                .Where(o => o.OkregId != randomDistrict.OkregId)
                .OrderBy(o => random.Next())
                .FirstOrDefault();

            if (neighboringDistrict != null && _radar.IsCountyNeighbouringWithDistrict(randomCounty, neighboringDistrict))
            {
                break;
            }
            else
            {
                neighboringDistrict = null;
            }
        }

        if (neighboringDistrict == null)
        {
            return neighborConfiguration; // No valid neighboring district found
        }

        randomDistrict.Powiaties.Remove(randomCounty);
        neighboringDistrict.Powiaties.Add(randomCounty);

        if (_radar.IsDistrictBoundaryUnbroken(randomDistrict) && _radar.IsDistrictBoundaryUnbroken(neighboringDistrict))
        {
            foreach (var dist in neighborConfiguration.Okregis)
            {
                if (_radar.IsDistrictBoundaryUnbroken(dist) && _codex.AreLegalRequirementsMet(neighborConfiguration, populationIndex))
                {
                    return neighborConfiguration; // New configuration is valid
                }
            }
            return RestoreOriginalConfiguration(neighborConfiguration, randomDistrict, neighboringDistrict, randomCounty);
        }

        return RestoreOriginalConfiguration(neighborConfiguration, randomDistrict, neighboringDistrict, randomCounty);
    }

    private Wojewodztwa RestoreOriginalConfiguration(Wojewodztwa config, Okregi randomDistrict, Okregi neighboringDistrict, Powiaty randomCounty)
    {
        randomDistrict.Powiaties.Add(randomCounty);
        neighboringDistrict.Powiaties.Remove(randomCounty);
        return config;
    }

    private Okregi SelectRandomDistrict(State state, Random random)
    {
        var districts = state.DistrictVotingResults!.Keys.ToList();
        return districts[random.Next(districts.Count)];
    }

    private Powiaty SelectRandomCounty(State state, Okregi destinationDistrict, Random random)
    {
        var destinationDistrictCounties = destinationDistrict.Powiaties.ToList();

        // Get all counties from the source district to select from
        var sourceCounties = state.ActualConfiguration!.Okregis
            .Where(o => o.Numer == destinationDistrict.Numer)
            .SelectMany(p => p.Powiaties)
            .ToList();

        if (sourceCounties.Count == 0) return null!;

        Powiaty selectedCounty;
        int maxAttempts = 100; // Limit attempts to avoid infinite loops
        int attempts = 0;

        // Repeatedly try to find a neighboring county
        do
        {
            selectedCounty = sourceCounties[random.Next(sourceCounties.Count)];
            attempts++;
        }
        while (attempts < maxAttempts &&
               !IsCountyNeighbouring(selectedCounty, destinationDistrictCounties));

        // Return null if no valid county is found after maximum attempts
        return attempts < maxAttempts ? selectedCounty : null!;
    }

    // Helper method to check if a county is neighboring any county in the destination district
    private bool IsCountyNeighbouring(Powiaty county, List<Powiaty> destinationCounties)
    {
        return destinationCounties.Any(destinationCounty =>
            _radar.AreCountiesNeighbouring(county, destinationCounty.PowiatId));
    }

    public State MoveCountyToAnotherDistrict(State state, Powiaty county, Okregi currentDistrict, Random random)
    {
        if (state == null || county == null || currentDistrict == null) throw new ArgumentNullException();
        var newDistrict = SelectDifferentRandomDistrict(state, currentDistrict, random);
        if (newDistrict == null) return state;
        
        // Check if moving the county would leave the current district empty
        if (CanMoveCounty(state, county, currentDistrict, newDistrict))
        {
            UpdateDistricts(state, county, currentDistrict, newDistrict);
        }
        return state;
    }

    private bool CanMoveCounty(State state, Powiaty county, Okregi currentDistrict, Okregi newDistrict)
    {
        // Check if moving the county would result in an empty current district
        var currentDistrictObj = state.ActualConfiguration!.Okregis.FirstOrDefault(o => o.OkregId == currentDistrict.OkregId);
        if (currentDistrictObj != null && currentDistrictObj.Powiaties.Count <= 1)
        {
            return false; // Cannot move if it would leave the district empty
        }
        return true;
    }

    private void UpdateDistricts(State state, Powiaty county, Okregi currentDistrict, Okregi newDistrict)
    {
        var currentDistrictObj = state.ActualConfiguration!.Okregis.FirstOrDefault(o => o.OkregId == currentDistrict.OkregId);
        var newDistrictObj = state.ActualConfiguration.Okregis.FirstOrDefault(o => o.OkregId == newDistrict.OkregId);

        if (currentDistrictObj != null) currentDistrictObj.Powiaties.Remove(county);
        newDistrictObj?.Powiaties.Add(county);
    }


    private Okregi SelectDifferentRandomDistrict(State state, Okregi currentDistrict, Random random)
    {
        var districts = state.ActualConfiguration!.Okregis.ToList();
        if (districts.Count <= 1) return null!;
        Okregi newDistrict;
        do
        {
            newDistrict = districts[random.Next(districts.Count)];
        } while (newDistrict.OkregId == currentDistrict.OkregId);

        return newDistrict;
    }
    public bool AreDistrictBoundariesValid(Okregi okregi)
    {
        if (okregi.Powiaties.Count == 0) return false;

        bool[] boundaryValidity = new bool[okregi.Powiaties.Count];

        for (int i = 0; i < okregi.Powiaties.Count; i++)
        {
            var currentPowiat = okregi.Powiaties.ToList()[i];
            boundaryValidity[i] = okregi.Powiaties
                .Any(powiat => !powiat.PowiatId.Equals(currentPowiat.PowiatId) &&
                    (_radar.AreCountiesNeighbouring(currentPowiat, powiat.PowiatId)));
        }

        return boundaryValidity.All(valid => valid);
    }

    private List<State> GenerateMultipleNeighbors(State currentState, double stepSize, int numberOfNeighbors)
    {
        var neighbors = new List<State>();
        for (int i = 0; i < numberOfNeighbors; i++)
        {
            neighbors.Add(GenerateNeighbor(currentState, stepSize));
        }
        return neighbors;
    }

    // Simulated Annealing Optimization
    public State Optimize(State initialState)
    {
        if (initialState == null) throw new ArgumentNullException(nameof(initialState));

        //From configuration static class
        double temperature = Configuration.InitialTemperature;
        double coolingRate = Configuration.CoolingRate;
        double stepSize = Configuration.StepSize;
        int maxIterations = Configuration.MaxIterations;
        var rand = new Random();

        State currentSolution = initialState;
        double currentObjective = EvaluateState(currentSolution);
        State bestSolution = currentSolution;
        double bestObjective = currentObjective;

        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            // Generate multiple neighbors and choose the one with the highest objective.
            var neighbors = GenerateMultipleNeighbors(currentSolution, stepSize, 5); // Generate 5 neighbors, for example.
            var bestNeighbor = neighbors.OrderByDescending(EvaluateState).First(); // Choose the neighbor with the highest objective value.
            double neighborObjective = EvaluateState(bestNeighbor);
            Console.WriteLine($"New score: {neighborObjective}");

            // Accept the best neighbor based on objective and temperature
            if (neighborObjective > currentObjective ||
                rand.NextDouble() < Math.Exp((neighborObjective - currentObjective) / temperature))
            {
                currentSolution = bestNeighbor;
                currentObjective = neighborObjective;
                if (currentObjective > bestObjective)
                {
                    
                    bestSolution = currentSolution;
                    bestObjective = currentObjective;
                }
            }
            // Update temperature based on the cooling rate
            temperature *= coolingRate;
        }
        return bestSolution;

    }
}
