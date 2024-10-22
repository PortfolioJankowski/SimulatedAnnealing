using Microsoft.AspNetCore.Mvc;
using SimulatedAnnealing.Server.Models.Algorithm.Variable;
using SimulatedAnnealing.Server.Services.Behavioral;

namespace SimulatedAnnealing.Server.Controllers
{
    [ApiController]
    [Route("api/Algorithm")]
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

    }
}
