using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimulatedAnnealing.Server.Models.Algorithm.Variable;
using SimulatedAnnealing.Server.Models.Requests;
using SimulatedAnnealing.Server.Services.Behavioral;
using System.Text;

namespace SimulatedAnnealing.Server.Controllers;

[ApiController]
[Route("api/Algorithm")]
[Authorize]
public class AlgorithmController : Controller
{
    private readonly ILogger<AlgorithmController> _logger;
    private readonly SimulatedAnnealingService _simulatedAnnealingService;
    private readonly IValidator<OptimizeLocalDistrictsRequest> _validator;
    public AlgorithmController(ILogger<AlgorithmController> logger, SimulatedAnnealingService simulatedAnnealingService, IValidator<OptimizeLocalDistrictsRequest> validator)
    {
        _logger = logger;
        _simulatedAnnealingService = simulatedAnnealingService;
        _validator = validator;
    }

    [HttpPost("OptimizeLocal")]
    public async Task<ActionResult<VoivodeshipState>> GetOptimisedVoivodeship([FromBody] OptimizeLocalDistrictsRequest districtsRequest)
    {
        var validationResult = _validator.Validate(districtsRequest);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { message = errors });
        }
        
        var optimized = await _simulatedAnnealingService.Optimize(districtsRequest);
        return Ok(optimized);
    }

    [HttpGet("test")]
    public string AuthorizationCheck()
    {
        //eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6ImV4Y2VsbGVuY2phQG8yLnBsIiwiZ2l2ZW5fbmFtZSI6IkV4Y2VsbGVuY2phIiwibmJmIjoxNzM4NDIyNTE5LCJleHAiOjE3MzkwMjczMTksImlhdCI6MTczODQyMjUxOSwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MjEwIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MjEwIn0.M2uZQhRMulFV1JE-QbGkFR4pp8xpM_HZ_8Vf2lJKvL0
        return "Test";
    }

}

