using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SimulatedAnnealing.Server.Models.Algorithm;
using SimulatedAnnealing.Server.Models.Algorithm.Fixed;
using SimulatedAnnealing.Server.Models.Algorithm.Variable;
using SimulatedAnnealing.Server.Models.DTOs;
using SimulatedAnnealing.Server.Models.Exceptions;

namespace SimulatedAnnealing.Server.Services.Database;

public class DbRepository : IDbRepository
{
    private readonly PhdApiContext _context;
    private readonly IOptions<AvailableDistricsOptions> _availableDistricts;
    private readonly ILogger _logger;
    private readonly IDistributedCache _distributedCache;

    public DbRepository(PhdApiContext context, ILogger<DbRepository> logger,
                        IOptions<AvailableDistricsOptions> availableDistricts, IDistributedCache distributedCache)
    {
        _context = context;
        _logger = logger;
        _availableDistricts = availableDistricts;
        _distributedCache = distributedCache;
    }

    public async Task<Voivodeship?> GetVoivodeshipAsync(InitialStateRequest request)
    {
        string key = $"voivodeship-{request.VoivodeshipName}-{request.Year}";

        return await _distributedCache.GetOrCreateAsync(key, async token =>
        {
            return await GetVoivodeshipFromDatabaseAsync(request);
        });
    }
    public async Task<GerrymanderingResult?> GetGerrymanderringResultsAsync(LocalResultsRequest request)
    {
        string key = $"results-{request.VoivodeshipName}-{request.Year}-{request.PoliticalParty}";

        return await _distributedCache.GetOrCreateAsync(key, async token =>
        {
            return await GetGerrymanderringResultsFromDatabaseAsync(request);
        });
    }

    public async Task<Voivodeship> GetVoivodeShipClone(Voivodeship toClone, int year)
    {
        string key = $"voivodeship-{toClone.Name}-{year}";
        var cachedJson = await _distributedCache.GetStringAsync(key);

        if (string.IsNullOrEmpty(cachedJson))
            throw new VoivodeshipNotFoundException($"No cached Voivodeship found for {toClone.Name}");

        var blueprint = JsonConvert.DeserializeObject<Voivodeship>(cachedJson);
        return blueprint!.DeepClone(blueprint, toClone);
    }

    private async Task<GerrymanderingResult?> GetGerrymanderringResultsFromDatabaseAsync(LocalResultsRequest request)
    {
        try
        {
            var results = await _context.GerrymanderingResults
                                    .Where(r => r.ChoosenParty == request.PoliticalParty
                                                && r.ElectoralYear == request.Year
                                                && r.Voivodeship == request.VoivodeshipName)
                                    .OrderByDescending(r => r.FinalScore)
                                    .FirstAsync();
            if (results == null)
                throw new GerrymanderringResultsNotFoundException($"Gerrymanderring result not found for " +
                    $"{request.PoliticalParty} in {request.Year} located in {request.VoivodeshipName}");

            return results;
        }
        catch (GerrymanderringResultsNotFoundException e)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching gerrymandering results for " +
                $"{request.VoivodeshipName} in {request.Year} located in {request.VoivodeshipName}");
            throw;
        }

    }

    private async Task<Voivodeship?> GetVoivodeshipFromDatabaseAsync(InitialStateRequest request)
    {
        try
        {
            var bestParties = GetBestPartiesFromConfig(request);
            var voivodeship = _context.Voivodeships
                .AsNoTracking()
                .Where(v => v.Name == request.VoivodeshipName)
                .Include(v => v.Districts)
                    .ThenInclude(d => d.Counties)
                        .ThenInclude(c => c.VotingResults
                            .Where(r => r.Year == request.Year && bestParties.Contains(r.Committee!)))
                .AsQueryable();

            var currentVoivodeship = await voivodeship.FirstOrDefaultAsync();

            if (currentVoivodeship == null)
                throw new VoivodeshipNotFoundException($"Voivodeship '{request.VoivodeshipName}' for year {request.Year} not found.");

            return await GetVoivodeshipNeighborsAsync(currentVoivodeship);
        }
        catch (VoivodeshipNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching voivodeship '{VoivodeshipName}' for year {Year}.", request.VoivodeshipName, request.Year);
            throw;
        }
    }

    private async Task<Voivodeship> GetVoivodeshipNeighborsAsync(Voivodeship voivodeship)
    {
        var neighbors = await _context.Neighbors.AsQueryable().ToListAsync();
        var counties = await _context.Counties.AsQueryable().ToListAsync();

        var neighborLookup = neighbors
                .GroupBy(n => n.CountyId)
                .ToDictionary(g => g.Key, g => g.Select(n => n.NeighborId).ToList());

        voivodeship.Districts
            .SelectMany(district => district.Counties)
            .ToList()
            .ForEach(county =>
            {
                if (neighborLookup.TryGetValue(county.CountyId, out var neighborIds))
                {
                    county.NeighboringCounties = counties
                        .Where(c => neighborIds.Contains(c.CountyId))
                        .ToList();
                }
            });

        return voivodeship;
    }

    private List<string> GetBestPartiesFromConfig(InitialStateRequest request)
    {
        var districts = _availableDistricts.Value.Districts;
        var bestParties = districts[request.VoivodeshipName.ToLower()][request.Year.ToString()].ToList();
        return bestParties;
    }



}