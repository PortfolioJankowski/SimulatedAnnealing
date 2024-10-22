using Microsoft.EntityFrameworkCore;
using SimulatedAnnealing.Server.Models.Algorithm.Fixed;
using System.Configuration;
namespace SimulatedAnnealing.Server.Services.Database;

public class CacheService
{
    private readonly PhdApiContext _context;
    private readonly IConfiguration _configuration;
    private readonly string _bestParties = "BestPartiesLocal";
    private readonly ILogger<CacheService> _logger;
    private IQueryable<Voivodeship>? _cachedVoivodeship;
    private IQueryable<County>? _cachedCounties;
    public CacheService(PhdApiContext context, IConfiguration configuration, ILogger<CacheService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public IQueryable<Voivodeship> GetVoivodeshipQueryable()
    {
        return _cachedVoivodeship!;
    }

    public IQueryable<County> GetCountyQueryable()
    {
        return _cachedCounties;
    }

    public void FetchCountyData()
    {
        _cachedCounties = _context.Counties;
    }
    public void FetchVoivodeshipData(int selectedYear, string selectedVoivodenship)
    {
        var bestPartiesSection = _configuration.GetSection(string.Concat(_bestParties, ":", selectedYear.ToString(), selectedVoivodenship.ToLower()));
        var bestParties = bestPartiesSection.Get<List<string>>();
        if (bestParties == null || !bestParties.Any())
        {
            var errorMessage = "Provided data is incorrect!";
            _logger.LogError(errorMessage);
            throw new Exception(errorMessage);
        }
        try
        {
            _cachedVoivodeship = _context.Voivodeships
           .Include(v => v.Districts)
           .ThenInclude(d => d.Counties)
           .ThenInclude(c => c.VotingResults
               .Where(r => r.Year == selectedYear && bestParties.Contains(r.Committee!)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching the voivodeship data.");
            throw new Exception($"An error occurred while fetching the voivodeship data: {ex.Message}");
        }

    }
}
