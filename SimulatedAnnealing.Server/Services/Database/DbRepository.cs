using Microsoft.EntityFrameworkCore;
using SimulatedAnnealing.Server.Models.Algorithm.Fixed;
using SimulatedAnnealing.Server.Models.DTOs;

namespace SimulatedAnnealing.Server.Services.Database;

public class DbRepository : IDbRepository
{
    private readonly PhdApiContext _context;
    private readonly ILogger _logger;

    public DbRepository(PhdApiContext context, ILogger<DbRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Voivodeship?> GetVoivodeship(ConfigurationQuery request)
    {
        var currentVoivodeShip = Cache.GetVoivodeshipQueryable(request).FirstOrDefault(); 
        var currentCounties = Cache.GetCountyQueryable();
        var neighbors = Cache.GetNeighborsQueryable();

        if (currentVoivodeShip == null)
            return null;

        var districts = currentVoivodeShip.Districts;
        foreach (var district in districts)
        {
            foreach (var county in district.Counties)
            {
                var neighborsIds = neighbors
                    .Where(n => n.CountyId == county.CountyId)
                    .Select(s => s.NeighborId)
                    .ToList();

                county.NeighboringCounties = currentCounties
                    .Where(c => neighborsIds.Contains(c.CountyId))
                    .ToList();
            }
        }
        return currentVoivodeShip;
    }


    public async Task<GerrymanderingResult?> GetLocalResults(LocalResultsQuery request)
    {
        try
        {
            return await _context.GerrymanderingResults
                .Where(r => r.ChoosenParty == request.PoliticalParty
                            && r.ElectoralYear == request.Year
                            && r.Voivodeship == request.Voivodeship)
                .OrderByDescending(r => r.FinalScore)
                .FirstAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving local results for Party: {request.PoliticalParty}, Year: {request.Year}, Voivodeship: {request.Voivodeship}", ex.Message);
            return null; 
        }
    }

}

