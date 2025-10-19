using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimulatedAnnealing.Server.Models.JNP;
using SimulatedAnnealing.Server.Services;

namespace SimulatedAnnealing.Server.Controllers
{
    [ApiController]
    [Route("api/jnp")]
    [Authorize]
    public class JNPController(
        JnpService jnpService,
        IValidator<CheckJnpRequest> validator)
        : Controller
    {
        [HttpPost("parliament-jnp")]
        public async Task<ActionResult<CheckJnpResponse>> IsJnpValid(
            [FromBody] CheckJnpRequest request)
        {
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var results = await jnpService.CheckParliamentJnpAsync(request.Year);
            return Ok(results);
        }
    }
}
