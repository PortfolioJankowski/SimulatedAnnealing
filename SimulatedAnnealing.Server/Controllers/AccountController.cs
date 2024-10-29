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

       //using tuple because async methods cannot have ref in or out parameters (thats why I couldnt use (...out var user, out var exception) in parameters
       var (user, exception) = await _userService.TryLoginUserAsync(loginDto);
       if (exception != null)
            return Unauthorized(exception.Message);

        return Ok(user); 
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (newUser, exception) = await _userService.TryRegisterAsync(registerDto);
        if (exception != null)
            return BadRequest(exception.Message);

        return Ok(newUser);   
    }    
}

