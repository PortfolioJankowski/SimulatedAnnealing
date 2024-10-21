using Microsoft.AspNetCore.Mvc;

namespace SimulatedAnnealing.Server.Controllers
{
    [ApiController]
    [Route("api/Algorithm")]
    public class AlgorithmController : Controller
    {
        private readonly ILogger<AlgorithmController> _logger;
        public AlgorithmController(ILogger<AlgorithmController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public string LoggerCheck()
        {
            _logger.LogInformation("Logs working.");
            return "siema";
        }
    }
}
