using SimulatedAnnealing.Server.Models.Algorithm.Fixed;
using SimulatedAnnealing.Server.Models.Algorithm.Fixed.Parliament;
using SimulatedAnnealing.Server.Models.Algorithm.Variable;
using SimulatedAnnealing.Server.Models.Configuration;
using SimulatedAnnealing.Server.Models.Requests;
using SimulatedAnnealing.Server.Services.Algorithm;
using SimulatedAnnealing.Server.Services.Configuration;
using SimulatedAnnealing.Server.Services.Database;

namespace SimulatedAnnealing.Server.Services.Behavioral;
public partial class SimulatedAnnealingService
{
    private readonly ComplianceService _complianceService;
    private readonly IDbRepository _dbRepository;
    private readonly VoivodeshipStateBuilder _stateBuilder;
    private readonly Random _random;
    private readonly Geolocator _geolocator;
    private readonly IConfiguration _configuration;
    private const int RANDOM_COUNTIES_GENERATED_PER_ITERATION = 5;
    private readonly AlgorithmConfigurationBuilder _algorithmConfigurationBuilder;
    private bool _isParliament = false;

    public SimulatedAnnealingService(ComplianceService complianceService, IDbRepository dbRepository,
        VoivodeshipStateBuilder stateBuilder, IConfiguration configuration,
        AlgorithmConfigurationBuilder algorithmConfigurationBuilder)
    {
        _complianceService = complianceService;
        _dbRepository = dbRepository;
        _stateBuilder = stateBuilder;
        _random = new Random();
        _geolocator = new Geolocator();
        _configuration = configuration;
        _algorithmConfigurationBuilder = algorithmConfigurationBuilder;
    }

    public async Task<LocalOptimizedResults> OptimizeParliamentDistrictRequest(OptimizeParliamentDistrictRequest request)
    {
        _isParliament = true;
        var districtInformation = request.ToLocalResutlsRequest();
        var localRequest = new OptimizeLocalDistrictsRequest
        {
            DistrictInformation = districtInformation
        };

        var algorithmConfiguration = _algorithmConfigurationBuilder.Build(localRequest.DistrictInformation, true);
        algorithmConfiguration.MaxIterations = 100;


        _stateBuilder.SetParliament(_isParliament);
        var currentSolution = _stateBuilder
            .SetVoivodeship(localRequest.DistrictInformation).Result
            .CalculateInhabitants()
            .CalculateVoievodianshipSeatsAmount()
            .CalculatePopulationIndex()
            .CalculateDistrictResults(localRequest.DistrictInformation.PoliticalParty)
            .CalculateScore(localRequest, algorithmConfiguration)
            .Build();


        var initialResult = GetInitialStateResults(currentSolution, _isParliament);    
        var initialScore = currentSolution.Indicator!.Score;
        var bestSolution = currentSolution;
        var bestObjective = initialScore;

        int neighboursAmount = RANDOM_COUNTIES_GENERATED_PER_ITERATION;
        var currentObjective = currentSolution.Indicator!.Score;
        var bestRandomSolutionObjective = 0d;

        for (int i = 0; i < algorithmConfiguration.MaxIterations; i++)
        {
            var randomStates = await GenerateRandomSolutions(currentSolution, algorithmConfiguration.StepSize, neighboursAmount, localRequest.DistrictInformation.Year);
            var bestRandomSolution = GetBestRandomSolution(randomStates, localRequest, algorithmConfiguration);
            bestRandomSolutionObjective = bestRandomSolution.Indicator!.Score;

            if (bestRandomSolutionObjective > currentObjective ||
                _random.NextDouble() < Math.Exp((bestRandomSolutionObjective - currentObjective) / algorithmConfiguration.Temperature))
            {
                currentSolution = bestRandomSolution;
                currentObjective = bestRandomSolutionObjective;

                if (currentObjective > bestObjective)
                {
                    bestSolution = currentSolution;
                    bestObjective = currentObjective;
                }
            }
            algorithmConfiguration.Temperature *= algorithmConfiguration.CoolingRate;
        }

        var algorithmResults = GetAlgorithmResults(bestSolution);
        return new LocalOptimizedResults()
        {
            StartScore = initialScore,
            InitialResult = initialResult,
            OptimizedResults = algorithmResults.Item1,
            NewConfiguration = algorithmResults.Item2,
            OptimizedScore = bestSolution.Indicator.Score,
            VoivodeshipName = localRequest.DistrictInformation.VoivodeshipName
        };
    }


    public async Task<LocalOptimizedResults> OptimizeLocal(OptimizeLocalDistrictsRequest request)
    {
        var algorithmConfiguration = _algorithmConfigurationBuilder.Build(request.DistrictInformation);
        algorithmConfiguration.MaxIterations = 100;

        var currentSolution = _stateBuilder
            .SetVoivodeship(request.DistrictInformation).Result
            .CalculateInhabitants()
            .CalculateVoievodianshipSeatsAmount()
            .CalculatePopulationIndex()
            .CalculateDistrictResults(request.DistrictInformation.PoliticalParty)
            .CalculateScore(request, algorithmConfiguration)
            .Build();

        var initialAlgorithmResult = GetAlgorithmResults(currentSolution);
        var initialResult = GetInitialStateResults(currentSolution);    //for reaserch purpose
        var initialScore = currentSolution.Indicator!.Score;
        var bestSolution = currentSolution;
        var bestObjective = initialScore;

        int neighboursAmount = RANDOM_COUNTIES_GENERATED_PER_ITERATION;
        var currentObjective = currentSolution.Indicator!.Score;
        var bestRandomSolutionObjective = 0d;

        for (int i = 0; i < algorithmConfiguration.MaxIterations; i++)
        {
            var randomStates = await GenerateRandomSolutions(currentSolution, algorithmConfiguration.StepSize, neighboursAmount, request.DistrictInformation.Year);
            var bestRandomSolution = GetBestRandomSolution(randomStates, request, algorithmConfiguration);
            bestRandomSolutionObjective = bestRandomSolution.Indicator!.Score;

            if (bestRandomSolutionObjective > currentObjective ||
                _random.NextDouble() < Math.Exp((bestRandomSolutionObjective - currentObjective) / algorithmConfiguration.Temperature))
            {
                currentSolution = bestRandomSolution;
                currentObjective = bestRandomSolutionObjective;

                if (currentObjective > bestObjective)
                {
                    bestSolution = currentSolution;
                    bestObjective = currentObjective;
                }
            }
            algorithmConfiguration.Temperature *= algorithmConfiguration.CoolingRate;
        }

        var algorithmResults = GetAlgorithmResults(bestSolution);
        return new LocalOptimizedResults()
        {
            StartScore = initialScore,
            OptimizedScore = bestSolution.Indicator.Score,
            InitialResult = initialResult,
            OptimizedResults = algorithmResults.Item1,
            NewConfiguration = algorithmResults.Item2,
            VoivodeshipName = request.DistrictInformation.VoivodeshipName,
            InitialSeatsDistribution = initialAlgorithmResult.districtSeats,
            OptimizedSeatsDistribution = algorithmResults.districtSeats
        };
    }
    //returns comitees with number of votes and districts numbers with

    private static (
    Dictionary<string, int> totalSeats,
    Dictionary<int, IEnumerable<string>> generatedCounties,
    Dictionary<int, Dictionary<string, int>> districtSeats
    ) GetAlgorithmResults(VoivodeshipState voivodeshipState)
    {
        // suma mandatów w województwie (dla całych komitetów)
        Dictionary<string, int> seatsAmount = new Dictionary<string, int>();

        // mandaty w podziale na okręgi
        Dictionary<int, Dictionary<string, int>> districtSeats = new Dictionary<int, Dictionary<string, int>>();

        var results = voivodeshipState.DistrictVotingResults;
        foreach (var district in results!)
        {
            var districtId = district.Key;
            var districtResult = new Dictionary<string, int>();

            foreach (var value in district.Value)
            {
                // sumujemy do całości województwa
                if (seatsAmount.ContainsKey(value.Key))
                    seatsAmount[value.Key] += value.Value;
                else
                    seatsAmount.Add(value.Key, value.Value);

                // zapisujemy dla danego okręgu
                districtResult[value.Key] = value.Value;
            }

            districtSeats[districtId.DistrictId] = districtResult;
        }

        // okręgi → powiaty
        Dictionary<int, IEnumerable<string>> generatedCounties = new Dictionary<int, IEnumerable<string>>();
        foreach (var district in voivodeshipState.ActualConfiguration.Districts)
        {
            generatedCounties.Add(district.DistrictId, district.Counties.Select(c => c.Name).ToList());
        }

        return (seatsAmount, generatedCounties, districtSeats);
    }

    private static Dictionary<string, int> GetInitialStateResults(VoivodeshipState voivodeshipState, bool isParliament = false)
    {
        Dictionary<string, int> seatsAmount = new Dictionary<string, int>();

        if (isParliament)
        {
            var result = voivodeshipState.ParliamentDistrictVotingResults;
            foreach (var item in result)
            {
                foreach (var value in item.Value)
                {
                    if (seatsAmount.ContainsKey(value.Key))
                    {
                        seatsAmount[value.Key] += value.Value;
                    }
                    else
                    {
                        seatsAmount.Add(value.Key, value.Value);
                    }
                }
            }
        }
        else
        {
            var results = voivodeshipState.DistrictVotingResults;
            foreach (var item in results!)
            {
                foreach (var value in item.Value)
                {
                    if (seatsAmount.ContainsKey(value.Key))
                    {
                        seatsAmount[value.Key] += value.Value;
                    }
                    else
                    {
                        seatsAmount.Add(value.Key, value.Value);
                    }
                }
            }
        }

        return seatsAmount;
    }
    private VoivodeshipState GetBestRandomSolution(List<VoivodeshipState> randomStates, OptimizeLocalDistrictsRequest request, AlgorithmConfiguration config)
    {
        int maxSeats = 0;

        if (_isParliament)
        {
            var rs = randomStates.FirstOrDefault();
            foreach (var district in rs.ActualConfiguration.ParliamentDistricts)
            {
                if (ComplianceService.ParliamentDistrictsSeats2023.TryGetValue(district.Id, out int value))
                {
                    maxSeats += value;
                }
            }
        }
        else
        {
            maxSeats = _configuration.GetSection("DistrictsSeats").GetValue<int>(request.DistrictInformation.VoivodeshipName);
        }

        if (_isParliament)
        {
            randomStates
                .ForEach(state => state.ParliamentDistrictVotingResults = _complianceService.CalculateResultsForParliamentDistricts(state.ActualConfiguration, maxSeats, state.PopulationIndex, request.DistrictInformation.PoliticalParty));
        }
        else
        {
            randomStates
                .ForEach(state => state.DistrictVotingResults = _complianceService.CalculateResultsForDistricts(state.ActualConfiguration, maxSeats, state.PopulationIndex, request.DistrictInformation.PoliticalParty));
        }

        randomStates
            .ForEach(state => state.Indicator = IndicatorService.SetNewIndicator(state, request, config, _isParliament));

        return randomStates.OrderByDescending(state => state.Indicator!.Score).First();
    }

    private async Task<List<VoivodeshipState>> GenerateRandomSolutions(VoivodeshipState currentSolution, double stepsize, int neighboursAmount, int year)
    {
        var solutions = new List<VoivodeshipState>();
        for (int i = 0; i < neighboursAmount; i++)
        {
            solutions.Add(await GenerateSolution(currentSolution, stepsize, year));
        }
        return solutions;
    }

    private async Task<VoivodeshipState> GenerateSolution(VoivodeshipState voivodeshipState, double stepsize, int year)
    {
        var voivodeshipClone = await _dbRepository.GetVoivodeShipClone(voivodeshipState.ActualConfiguration, year);
        voivodeshipClone = await MoveRandomCounty(voivodeshipClone, voivodeshipState.PopulationIndex);

        if (!_complianceService.AreLegalRequirementsMet(voivodeshipClone, voivodeshipState.PopulationIndex, _isParliament))
        {
            return voivodeshipState; //Nothing changed
        }
        return GetVoivodeshipStateClone(voivodeshipState, voivodeshipClone);
    }

   

    private static VoivodeshipState GetVoivodeshipStateClone(VoivodeshipState voivodeshipState, Voivodeship voivodeshipClone)
    {
        return new VoivodeshipState
        {
            ActualConfiguration = voivodeshipClone,
            PopulationIndex = voivodeshipState.PopulationIndex,
            VoivodeshipInhabitants = voivodeshipState.VoivodeshipInhabitants,
            VoivodeshipSeatsAmount = voivodeshipState.VoivodeshipSeatsAmount,
        };
    }

    private async Task<Voivodeship> MoveRandomCounty(Voivodeship voivodeshipClone, double populationIndex)
    {
        if (_isParliament)
            return HandleParliamentCountyMoving(voivodeshipClone, populationIndex);

        return HandleLocalCountyMoving(voivodeshipClone, populationIndex);
    }

    private Voivodeship HandleLocalCountyMoving(Voivodeship voivodeshipClone, double populationIndex)
    {
        var randomDistrict = voivodeshipClone.Districts.OrderBy(d => _random.Next()).First();
        var randomCounty = randomDistrict.Counties.OrderBy(c => _random.Next()).First();
        District neighboringDistrict = null;
        int maxCountyShiftsAmount = 100;
        int attempt = 0;

        while (neighboringDistrict is null && attempt < maxCountyShiftsAmount)
        {
            attempt++;
            neighboringDistrict = voivodeshipClone.Districts
                                        .Where(d => d.DistrictId != randomDistrict.DistrictId)
                                        .OrderBy(d => _random.Next())
                                        .FirstOrDefault()!;

            if (neighboringDistrict != null && Geolocator.IsCountyNeighbouringWithDistrict(randomCounty, neighboringDistrict))
            {
                break;
            }
            else
            {
                neighboringDistrict = null;
            }
        }
        if (neighboringDistrict is null)
            return voivodeshipClone; //No valid neighboring district found

        randomDistrict.Counties.Remove(randomCounty);
        neighboringDistrict.Counties.Add(randomCounty);

        if (AreDistrictsValid(voivodeshipClone, neighboringDistrict, randomDistrict, populationIndex))
            return voivodeshipClone;

        return RestoreOriginalConfiguration(voivodeshipClone, randomDistrict, neighboringDistrict, randomCounty);
    }

    private Voivodeship HandleParliamentCountyMoving(Voivodeship voivodeshipClone, double populationIndex)
    {
        var randomDistr = voivodeshipClone.ParliamentDistricts.OrderBy(d => _random.Next()).First();
        var randomCounty = randomDistr.TerytCounties.OrderBy(c => _random.Next()).First();
        ParliamentDistrict neighboringDistrict = null;
        int maxCountyShiftsAmount = 100;
        int attempt = 0;

        while (neighboringDistrict is null && attempt < maxCountyShiftsAmount)
        {
            attempt++;
            neighboringDistrict = voivodeshipClone.ParliamentDistricts
                .Where(d => d.Id != randomDistr.Id)
                .OrderBy(d => _random.Next())
                .FirstOrDefault()!;

            if (neighboringDistrict != null && Geolocator.IsCountyNeighbouringWithDistrictParliament(randomCounty, neighboringDistrict))
            {
                break;
            }
            else
            {
                neighboringDistrict = null;
            }
        }
        if (neighboringDistrict is null)
            return voivodeshipClone;

        randomDistr.TerytCounties.Remove(randomCounty);
        neighboringDistrict.TerytCounties.Add(randomCounty);

        if (AreDistrictsValid(voivodeshipClone, neighboringDistrict, randomDistr, populationIndex))
            return voivodeshipClone;

        return RestoreOriginalConfiguration(voivodeshipClone, randomDistr, neighboringDistrict, randomCounty);
    }

    private bool AreDistrictsValid(Voivodeship voivodeship, District neighboringDistrict, District randomDistrict, double populationIndex)
    {
        if (_geolocator.IsDistrictBoundariesUnbroken(randomDistrict) && _geolocator.IsDistrictBoundariesUnbroken(neighboringDistrict))
        {
            foreach (var dist in voivodeship.Districts)
            {
                if (_geolocator.IsDistrictBoundariesUnbroken(dist) && _complianceService.AreLegalRequirementsMet(voivodeship, populationIndex))
                {
                    return true; //New configuration is Valid!
                }
            }
            return false;
        }
        return false;
    }

    private bool AreDistrictsValid(Voivodeship voivodeship, ParliamentDistrict neighboringDistrict, ParliamentDistrict randomDistrict, double populationIndex)
    {
        if (_geolocator.IsParliamentDistrictBoundariesUnbroken(randomDistrict) && _geolocator.IsParliamentDistrictBoundariesUnbroken(neighboringDistrict))
        {
            foreach (var dist in voivodeship.Districts)
            {
                if (_geolocator.IsDistrictBoundariesUnbroken(dist) && _complianceService.AreLegalRequirementsMet(voivodeship, populationIndex))
                {
                    return true; //New configuration is Valid!
                }
            }
            return false;
        }
        return false;
    }

    private Voivodeship RestoreOriginalConfiguration(Voivodeship config, District randomDistrict, District neighboringDistrict, County randomCounty)
    {
        randomDistrict.Counties.Add(randomCounty);
        neighboringDistrict.Counties.Remove(randomCounty);
        return config;
    }

    private Voivodeship RestoreOriginalConfiguration(Voivodeship config, ParliamentDistrict randomDistrict, ParliamentDistrict neighboringDistrict, TerytCounty randomCounty)
    {
        randomDistrict.TerytCounties.Add(randomCounty);
        neighboringDistrict.TerytCounties.Remove(randomCounty);
        return config;
    }
}
