using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimulatedAnnealing.Server.Models.Algorithm.Variable;
using SimulatedAnnealing.Server.Models.Requests;
using SimulatedAnnealing.Server.Services.Behavioral;
using SimulatedAnnealing.Server.Services.Configuration;

namespace SimulatedAnnealing.Server.Controllers;

[ApiController]
[Route("api/algorithm")]
[Authorize]
public class AlgorithmController(
    SimulatedAnnealingService simulatedAnnealingService, 
    IValidator<OptimizeLocalDistrictsRequest> localValidator,
    IValidator<OptimizeParliamentSeatsRequest> parliamentValidator
    ) : Controller
{

    [HttpPost("optimize-local")]
    public async Task<ActionResult<VoivodeshipState>> GetOptimisedVoivodeship(
        [FromBody] OptimizeLocalDistrictsRequest districtsRequest)
    {
        var validationResult = localValidator.Validate(districtsRequest);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { message = errors });
        }
        
        var optimized = await simulatedAnnealingService.Optimize(districtsRequest);
        return Ok(optimized);
    }

    [HttpPost("optimize-parliament")]
    public async Task<ActionResult<object>> GetOptimizedParliamentDistricts(
        [FromBody] OptimizeParliamentSeatsRequest parliamentRequest)
    {
        var validationResult = parliamentValidator.Validate(parliamentRequest);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { message = errors });
        }


        return Ok(new { Id = 1 });
    }

    [HttpGet("test")]
    public string AuthorizationCheck()
    {
        //eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6ImV4Y2VsbGVuY2phQG8yLnBsIiwiZ2l2ZW5fbmFtZSI6IkV4Y2VsbGVuY2phIiwibmJmIjoxNzM4NDIyNTE5LCJleHAiOjE3MzkwMjczMTksImlhdCI6MTczODQyMjUxOSwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MjEwIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MjEwIn0.M2uZQhRMulFV1JE-QbGkFR4pp8xpM_HZ_8Vf2lJKvL0
        return "Test";
    }

}

