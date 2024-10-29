using SimulatedAnnealing.Server.Models.Authentication;
using SimulatedAnnealing.Server.Models.Authentication.Dto;

namespace SimulatedAnnealing.Server.Services.Authentication;

public interface IUserService
{
    Task<(NewUserDto? newUser, Exception? exception)> TryRegisterAsync(RegisterDto registerDto);
    Task<(NewUserDto? user, Exception? exception)> TryLoginUserAsync(LoginDto loginDto);
}
