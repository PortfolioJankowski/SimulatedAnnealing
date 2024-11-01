using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SimulatedAnnealing.Server.Models.Algorithm.Fixed;
using SimulatedAnnealing.Server.Models.DTOs;
using SimulatedAnnealing.Server.Services.Database;

//TODO REFACTOR TEGO
public static class Cache
{
    private static readonly ConcurrentDictionary<string, IQueryable<Voivodeship>> _voivodeshipCache = new();
    private static readonly ConcurrentDictionary<string, IQueryable<County>> _countyCache = new();
    private static readonly ConcurrentDictionary<string, IQueryable<Neighbor>> _neighborCache = new();

    private static IConfiguration _configuration;
    private static ILogger _logger;
    private static PhdApiContext _context;
    private static readonly string _bestParties = "BestPartiesLocal";

    public static void Initialize(IConfiguration configuration, ILogger logger, PhdApiContext context)
    {
        _configuration = configuration;
        _logger = logger;
        _context = context;
    }

    public static IQueryable<Voivodeship> GetVoivodeshipQueryable(ConfigurationQuery request)
    {
        string cacheKey = $"{request.Year}-{request.VoivodeshipName.ToLower()}";

        return _voivodeshipCache.GetOrAdd(cacheKey, _ =>
        {
            return FetchVoivodeshipData(request);
        });
    }

    public static IQueryable<County> GetCountyQueryable()
    {
        return _countyCache.GetOrAdd("AllCounties", _ => FetchCountyData());
    }

    public static IQueryable<Neighbor> GetNeighborsQueryable()
    {
        return _neighborCache.GetOrAdd("AllNeighbors", _ => FetchNeighborsData());
    }

    private static IQueryable<Voivodeship> FetchVoivodeshipData(ConfigurationQuery request)
    {
        var bestPartiesSection = _configuration.GetSection($"{_bestParties}:{request.Year}:{request.VoivodeshipName.ToLower()}");
        var bestParties = bestPartiesSection.Get<List<string>>();

        if (bestParties == null || !bestParties.Any())
        {
            var errorMessage = "Provided data is incorrect!";
            _logger.LogError(errorMessage);
            throw new Exception(errorMessage);
        }

        try
        {
            return _context.Voivodeships
               .Where(v => v.Name == request.VoivodeshipName)
               .Include(v => v.Districts)
                    .ThenInclude(d => d.Counties)
                        .ThenInclude(c => c.VotingResults
                            .Where(r => r.Year == request.Year && bestParties.Contains(r.Committee!)))
               .AsQueryable();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching the voivodeship data.");
            throw new Exception($"An error occurred while fetching the voivodeship data: {ex.Message}");
        }
    }

    private static IQueryable<County> FetchCountyData()
    {
        return _context.Counties.Include(c => c.VotingResults).AsQueryable();
    }

    private static IQueryable<Neighbor> FetchNeighborsData()
    {
        return _context.Neighbors.AsQueryable();
    }

    public static void ClearCache()
    {
        _voivodeshipCache.Clear();
        _countyCache.Clear();
        _neighborCache.Clear();
    }
}
