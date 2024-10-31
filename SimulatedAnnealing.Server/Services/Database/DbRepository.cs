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
    public async Task<GerrymanderingResult?> GetLocalResults(GetLocalResultsRequest request)
    {
        try
        {
            return await _context.GerrymanderingResults
                .Where(r => r.ChoosenParty == request.PoliticalParty
                            && r.ElectoralYear == request.Year
                            && r.Voivodeship == request.Voivodeship)
                .OrderByDescending(r => r.FinalScore)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving local results for Party: {request.PoliticalParty}, Year: {request.Year}, Voivodeship: {request.Voivodeship}", ex.Message);
            return null; 
        }
    }

}

