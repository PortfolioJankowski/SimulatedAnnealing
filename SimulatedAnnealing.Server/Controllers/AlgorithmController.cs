using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimulatedAnnealing.Server.Models.Algorithm.Variable;
using SimulatedAnnealing.Server.Services.Behavioral;

namespace SimulatedAnnealing.Server.Controllers
{
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

        [HttpGet]
        public State GetOptimisedVoivodeship()
        {
            return _simulatedAnnealingService.Optimize();
        }

        [HttpGet("test")]
        public string AuthorizationCheck()
        {
            //nie zabij mnie za to, ale TOKEN DO SWAGGERA XDDDDDDDDDDDDDDDDDD: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6Im1hdGlAZXhhbXBsZS5jb20iLCJnaXZlbl9uYW1lIjoicGhkTWF0aSIsIm5iZiI6MTcyOTYzMTEzMywiZXhwIjoxNzMwMjM5NTMzLCJpYXQiOjE3Mjk2MzExMzMsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTIxMCIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTIxMCJ9.p5SlCe87t74c2MhtFvuQOFDjT5_RWhjQDtLIrX7E3jQ
            return "Test";
        }

    }
}
