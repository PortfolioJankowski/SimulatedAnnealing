using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimulatedAnnealing.Server.Models.Algorithm.Variable;
using SimulatedAnnealing.Server.Services.Behavioral;

namespace SimulatedAnnealing.Server.Controllers;

[ApiController]
[Route("api/Algorithm")]
[Authorize]
public class AlgorithmController : Controller
{
    private readonly ILogger<AlgorithmController> _logger;
    private readonly SimulatedAnnealingService _simulatedAnnealingService;
    public AlgorithmController(ILogger<AlgorithmController> logger, SimulatedAnnealingService simulatedAnnealingService)
    {
        _logger = logger;
        _simulatedAnnealingService = simulatedAnnealingService;
    }

    [HttpGet("Optimize")]
    public DistrictState GetOptimisedVoivodeship()
    {
        return _simulatedAnnealingService.Optimize();
    }

    [HttpGet("test")]
    public string AuthorizationCheck()
    {
        //eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6ImV4Y2VsbGVuY2phQG8yLnBsIiwiZ2l2ZW5fbmFtZSI6IkV4Y2VsbGVuY2phIiwibmJmIjoxNzM4NDIyNTE5LCJleHAiOjE3MzkwMjczMTksImlhdCI6MTczODQyMjUxOSwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MjEwIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MjEwIn0.M2uZQhRMulFV1JE-QbGkFR4pp8xpM_HZ_8Vf2lJKvL0
        return "Test";
    }

}

