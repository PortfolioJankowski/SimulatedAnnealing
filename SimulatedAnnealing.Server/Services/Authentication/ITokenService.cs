using SimulatedAnnealing.Server.Models.Authentication;

namespace SimulatedAnnealing.Server.Services.Authentication;
public interface ITokenService
{
    string CreateToken(AppUser appUser);
}

