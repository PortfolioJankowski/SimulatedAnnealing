using SimulatedAnnealing.Server.Models.Algorithm.Fixed;
using SimulatedAnnealing.Server.Models.DTOs;

namespace SimulatedAnnealing.Server.Services.Database;

public interface IDbRepository
{
    Task<GerrymanderingResult?> GetLocalResultsAsync(LocalResultsRequestBody request);
    Task<Voivodeship?> GetVoivodeshipAsync(ConfigurationRequestBody request);
}
