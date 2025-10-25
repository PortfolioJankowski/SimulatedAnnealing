using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimulatedAnnealing.Server.Models.Requests;
using SimulatedAnnealing.Server.Services;
using SimulatedAnnealing.Server.Services.Behavioral;
using SimulatedAnnealing.Server.Services.Database;
using SimulatedAnnealing.Server.Services.Extensions;

namespace SimulatedAnnealing.Server.Controllers;

[ApiController]
[Route("api/algorithm")]
[Authorize]
public class AlgorithmController(
    SimulatedAnnealingService simulatedAnnealingService,
    IValidator<OptimizeLocalDistrictsRequest> localValidator,
    IValidator<OptimizeParliamentSeatsRequest> parliamentValidator,
    ParliamentSeatAllocationService allocationService,
    PhdApiContext dbContext
    ) : Controller
{

    [HttpPost("optimize-local")]
    public async Task<ActionResult<LocalOptimizedResults>> GetOptimisedVoivodeship(
        [FromBody] OptimizeLocalDistrictsRequest districtsRequest)
    {
        var validationResult = localValidator.Validate(districtsRequest);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { message = errors });
        }

        var optimized = await simulatedAnnealingService.OptimizeLocal(districtsRequest);
        return Ok(optimized);
    }

    [HttpPost("optimize-parliament")]
    public async Task<ActionResult<ParliamentResults>> GetOptimizedParliamentDistricts(
        [FromBody] OptimizeParliamentSeatsRequest parliamentRequest)
    {
        var validationResult = parliamentValidator.Validate(parliamentRequest);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { message = errors });
        }

        var vvs = dbContext.Voivodeships
            .Include(v => v.ParliamentDistricts)
            .ThenInclude(d => d.TerytCounties).ThenInclude(c => c.CountyPopulations)
            .ToList();

        var okregi = vvs.SelectMany(v => v.ParliamentDistricts).ToList();
        await allocationService.AllocateSeatsAsync(okregi, parliamentRequest.year);

        List<LocalOptimizedResults> optimizedResultsList = new();

        foreach (var v in vvs)
        {
            var okregiWWojewodztwie = okregi
                .Where(d => d.VoivodeshipId == v.VoivodeshipsId)
                .ToList();

             Dictionary<int, int> seatsByDistrict = new();
             okregiWWojewodztwie.ForEach(d =>
             {
                 seatsByDistrict[d.Id] = d.SeatsToAllocate;
             });

            var request = new OptimizeParliamentDistrictRequest()
            {
                PoliticalParty = parliamentRequest.comittee.GetDescription(),
                VoivodeshipName = v.Name,
                Year = parliamentRequest.year
            };

            optimizedResultsList.Add(await simulatedAnnealingService.OptimizeParliamentDistrictRequest(request, seatsByDistrict));
        }

        //AGREGACJA WYNIKÓW Z CZĘŚCI LOKALNYCH DO JEDNEGO OBIEKTU PARLIAMENTRESULTS
        double totalInitialScore = optimizedResultsList.Sum(r => r.StartScore);
        double totalOptimizedScore = optimizedResultsList.Sum(r => r.OptimizedScore);

        var initialSeatsByParties = optimizedResultsList
            .SelectMany(r => r.InitialResult)
            .GroupBy(pair => pair.Key)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(pair => pair.Value)
            );

        var optimizedSeatsByParties = optimizedResultsList
            .SelectMany(r => r.OptimizedResults)
            .GroupBy(pair => pair.Key)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(pair => pair.Value)
            );

        double totalInitialSeats = initialSeatsByParties.Sum(p => (double)p.Value);
        double totalOptimizedSeats = optimizedSeatsByParties.Sum(p => (double)p.Value);

        var finalResults = new ParliamentResults
        {
            InitialSeats = totalInitialSeats,
            OptimizedSeats = totalOptimizedSeats,
            InitialSeatsByParties = initialSeatsByParties,
            OptimizedSeatsByParties = optimizedSeatsByParties,
            InitialScore = totalInitialScore,
            OptimizedScore = totalOptimizedScore,
            optimizedResults = optimizedResultsList
        };

        return Ok(finalResults);
    }

    [HttpGet("test")]
    public string AuthorizationCheck()
    {
        //eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6ImV4Y2VsbGVuY2phQG8yLnBsIiwiZ2l2ZW5fbmFtZSI6IkV4Y2VsbGVuY2phIiwibmJmIjoxNzM4NDIyNTE5LCJleHAiOjE3MzkwMjczMTksImlhdCI6MTczODQyMjUxOSwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MjEwIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MjEwIn0.M2uZQhRMulFV1JE-QbGkFR4pp8xpM_HZ_8Vf2lJKvL0
        return "Test";
    }

}

public class ParliamentSeatAllocationResult
{
    public string VoivodeshipName { get; set; }
    public Dictionary<string, int> SeatsByDistrict { get; set; }
}

public class ParliamentResults
{
    public double InitialSeats { get; set; }
    public double OptimizedSeats { get; set; }
    public  Dictionary<string, int> InitialSeatsByParties { get; set; }
    public Dictionary<string, int> OptimizedSeatsByParties { get; set; }
    public double InitialScore { get; set; }
    public double OptimizedScore { get; set; }
    public 
    List<LocalOptimizedResults> optimizedResults { get; set; }  
}

