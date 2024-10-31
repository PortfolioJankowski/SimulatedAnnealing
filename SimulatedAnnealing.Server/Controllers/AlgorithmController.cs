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
    public State GetOptimisedVoivodeship()
    {
        return _simulatedAnnealingService.Optimize();
    }

    [HttpGet("test")]
    public string AuthorizationCheck()
    {
        //eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InVzZXJAZXhhbXBsZS5jb20iLCJnaXZlbl9uYW1lIjoic2llbWEiLCJuYmYiOjE3MzAzOTMzNzgsImV4cCI6MTczMDk5ODE3OCwiaWF0IjoxNzMwMzkzMzc4LCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjUyMTAiLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjUyMTAifQ.d7uyonhDFcNg1DoaZiCKtmJ77N78jVB_YvuFjLEnsdY
        return "Test";
    }

}

