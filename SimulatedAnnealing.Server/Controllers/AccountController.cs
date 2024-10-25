using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimulatedAnnealing.Server.Models.Authentication;
using SimulatedAnnealing.Server.Models.Authentication.Dto;
using SimulatedAnnealing.Server.Models.Authentication.Exceptions;
using SimulatedAnnealing.Server.Services.Authentication;

namespace SimulatedAnnealing.Server.Controllers;
[Route("api/Account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IUserService _userService;
    
    public AccountController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var user = await _userService.LogInAsync(loginDto);
            return Ok(user);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred during login.");
        }     
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var newUser = await _userService.RegisterAsync(registerDto);
            return Ok(newUser);
        } catch (UserCreationException ex)
        {
            return BadRequest(new
            {
                Message = ex.Message,
                Errors = ex.Errors.Select(e => e.Description) 
            });
        } catch (Exception)
        {
            return StatusCode(500, "An error occured during registration");
        }
    }    
}

