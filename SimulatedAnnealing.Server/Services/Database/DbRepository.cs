using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimulatedAnnealing.Server.Models.Algorithm;
using SimulatedAnnealing.Server.Models.Algorithm.Fixed;
using SimulatedAnnealing.Server.Models.DTOs;

namespace SimulatedAnnealing.Server.Services.Database;

public class DbRepository : IDbRepository
{
    private readonly PhdApiContext _context;
    private readonly IOptions<AvailableDirstricsOptions> _availableDistricts;
    private readonly ILogger _logger;

    private readonly IConfiguration _configuration;

    public DbRepository(PhdApiContext context, ILogger<DbRepository> logger,  IOptions<AvailableDirstricsOptions> availableDistricts)
    {
        _context = context;
        _logger = logger;
        _availableDistricts = availableDistricts;
    }

    public async Task<Voivodeship?> GetVoivodeshipAsync(ConfigurationRequestBody request)
    {
        // Check the cache first
        var cachedVoivodeship = Cache.GetVoivodeshipQueryable();
        if (cachedVoivodeship != null)
            return await cachedVoivodeship.FirstOrDefaultAsync();

        // Load best parties from configuration
       //var bestParties = _configuration.GetSection($"{_bestParties}:{request.Year}:{request.VoivodeshipName.ToLower()}").Get<List<string>>();
        var districts = _availableDistricts.Value.Districts;
        var bestParties = districts[request.VoivodeshipName.ToLower()][request.Year.ToString()].ToList();

        try
        {
            // Fetch voivodeship and related data from the database
            var voivodeship = _context.Voivodeships
                .Where(v => v.Name == request.VoivodeshipName)
                .Include(v => v.Districts)
                    .ThenInclude(d => d.Counties)
                        .ThenInclude(c => c.VotingResults
                            .Where(r => r.Year == request.Year && bestParties.Contains(r.Committee!)))
                .AsQueryable();

            var currentVoivodeship = await voivodeship.FirstOrDefaultAsync();
            if (currentVoivodeship == null)
                 return await Task.FromResult<Voivodeship?>(null);

            var neighbors = await _context.Neighbors.AsQueryable().ToListAsync();
            var counties = await _context.Counties.AsQueryable().ToListAsync();

            // Set caches
            Cache.SetVoivodeshipIQueryable(voivodeship);
            Cache.SetCountiesIQueryable(counties.AsQueryable());
            Cache.SetNeighborsIQueryable(neighbors.AsQueryable());

            var neighborLookup = neighbors
                .GroupBy(n => n.CountyId)
                .ToDictionary(g => g.Key, g => g.Select(n => n.NeighborId).ToList());

            currentVoivodeship.Districts
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

            return currentVoivodeship;
        }
        catch (Exception ex)
        {
            var errorMessage = "An error occurred while fetching the voivodeship data.";
            _logger.LogError(ex, errorMessage);
            return await Task.FromResult<Voivodeship?>(null);
        }
    }

    public async Task<GerrymanderingResult?> GetGerrymanderringResults(LocalResultsRequestBody request)
    {
        try
        {
            return await _context.GerrymanderingResults
                .Where(r => r.ChoosenParty == request.PoliticalParty
                            && r.ElectoralYear == request.Year
                            && r.Voivodeship == request.VoivodeshipName)
                .OrderByDescending(r => r.FinalScore)
                .FirstAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving local results for Party: {request.PoliticalParty}, Year: {request.Year}, Voivodeship: {request.VoivodeshipName}", ex.Message);
            return null; 
        }
    }

}

