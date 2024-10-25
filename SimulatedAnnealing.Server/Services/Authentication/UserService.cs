using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimulatedAnnealing.Server.Models.Authentication;
using SimulatedAnnealing.Server.Models.Authentication.Dto;
using SimulatedAnnealing.Server.Models.Authentication.Exceptions;
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
        public async Task<NewUserDto?> LogInAsync(LoginDto loginDto)
        {
            var user = await _userManager.Users.FirstAsync(x => x.UserName == loginDto.Username.ToLower()); 
            if (user == null) 
                throw new UnauthorizedAccessException("Invalid username!");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded) 
                throw new UnauthorizedAccessException("Invalid password!");

            return new NewUserDto
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = _tokenService.CreateToken(user)
            };
        }

        public async Task<NewUserDto?> RegisterAsync(RegisterDto registerDto)
        {
            var appUser = new AppUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
            };
           var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

            if (!createdUser.Succeeded)
                throw new UserCreationException("User creation failed!", createdUser.Errors);

            var roleResult = await _userManager.AddToRoleAsync(appUser, "User"); //impossible to create Admin via Endpoint
            if (!roleResult.Succeeded)
                throw new UserCreationException("Role assignment failed!", roleResult.Errors);

            return new NewUserDto
            {
                Email = appUser.Email,
                UserName = appUser.UserName,
                Token = _tokenService.CreateToken(appUser)
            };
        }

      

        

        private async Task<AppUser> GetUserById(string username)
        {
            return await _userManager.Users.FirstAsync(x => x.UserName == username.ToLower());
        }


    }
}
