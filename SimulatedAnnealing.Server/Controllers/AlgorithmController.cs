using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimulatedAnnealing.Server.Models.DTOs;
using SimulatedAnnealing.Server.Models.Requests;
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
    PhdApiContext dbContext
    ) : Controller
{
    //TODO => LEPSZE POKAZANIE DANYCH. ŻEBY POKAZYWAŁO ILE OKRĘGÓW BYŁO I ILE JEST

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
    public async Task<ActionResult<List<LocalOptimizedResults>>> GetOptimizedParliamentDistricts(
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
                .ThenInclude(d => d.TerytCounties)
                    .ThenInclude(c => c.CountyPopulations)
             .Include(v => v.ParliamentDistricts)
                .ThenInclude(d => d.TerytCounties)
                    .ThenInclude(c => c.ParliamentVotingResults)
            .Include(v => v.ParliamentDistricts)
                .ThenInclude(d => d.TerytCounties)
                    .ThenInclude(c => c.TerytNeighborCountyTerytNavigations)
            .ToList();

        List<LocalOptimizedResults> optimizedResults = new();
        foreach (var v in vvs)
        {
            var request = new OptimizeParliamentDistrictRequest()
            {
                PoliticalParty = parliamentRequest.comittee.GetDescription(),
                VoivodeshipName = v.Name,
                Year = parliamentRequest.year
            };

            optimizedResults.Add(await simulatedAnnealingService.OptimizeParliamentDistrictRequest(request));
        }

        return Ok(optimizedResults);
    }

    [HttpGet("test")]
    public string AuthorizationCheck()
    {
        //eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6ImV4Y2VsbGVuY2phQG8yLnBsIiwiZ2l2ZW5fbmFtZSI6IkV4Y2VsbGVuY2phIiwibmJmIjoxNzM4NDIyNTE5LCJleHAiOjE3MzkwMjczMTksImlhdCI6MTczODQyMjUxOSwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MjEwIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MjEwIn0.M2uZQhRMulFV1JE-QbGkFR4pp8xpM_HZ_8Vf2lJKvL0
        return "Test";
    }

}

