using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimulatedAnnealing.Server.Models.Authentication;
using SimulatedAnnealing.Server.Models.Authentication.Dto;
using SimulatedAnnealing.Server.Models.Authentication.Exceptions;
using System;
using System.ComponentModel.DataAnnotations;

namespace SimulatedAnnealing.Server.Services.Authentication
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<(NewUserDto? newUser, Exception? exception)> TryRegisterAsync(RegisterDto registerDto)
        {
            var appUser = new AppUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
            };
            var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

            if (!createdUser.Succeeded)
                return (null, new UserCreationException(message: "User creation failed!", errors: createdUser.Errors));

            var roleResult = await _userManager.AddToRoleAsync(appUser, "User"); //impossible to create Admin via Endpoint
            if (!roleResult.Succeeded)
                return (null, new UserCreationException(message: "Role assignment failed!", errors: roleResult.Errors));

            var userResult = new NewUserDto
            {
                Email = appUser.Email,
                UserName = appUser.UserName,
                Token = _tokenService.CreateToken(appUser)
            };
            return (userResult, null);
        }

        public async Task<(NewUserDto? user, Exception? exception)> TryLoginUserAsync(LoginDto loginDto)
        {
            var appUser = await _userManager.Users.FirstAsync(x => x.UserName == loginDto.Username.ToLower());
            if (appUser == null)
                return (null, new UnauthorizedAccessException("Invalid username!"));

            var result = await _signInManager.CheckPasswordSignInAsync(appUser, loginDto.Password, false);
            if (!result.Succeeded)
                return (null, new UnauthorizedAccessException("Invalid password!"));

            var userDto  = new NewUserDto
            {
                UserName = appUser.UserName,
                Email = appUser.Email,
                Token = _tokenService.CreateToken(appUser)
            };
            return (userDto, null);
        }

        private async Task<AppUser> GetUserById(string username)
        {
            return await _userManager.Users.FirstAsync(x => x.UserName == username.ToLower());
        }


    }
}
