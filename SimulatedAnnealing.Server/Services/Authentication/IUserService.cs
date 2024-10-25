using SimulatedAnnealing.Server.Models.Authentication;
using SimulatedAnnealing.Server.Models.Authentication.Dto;

namespace SimulatedAnnealing.Server.Services.Authentication
{
    public interface IUserService
    {
        Task<NewUserDto?> LogInAsync(LoginDto loginDto);
        Task<NewUserDto?> RegisterAsync(RegisterDto registerDto);

    }
}