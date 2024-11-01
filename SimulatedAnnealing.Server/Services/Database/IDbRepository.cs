using SimulatedAnnealing.Server.Models.Algorithm.Fixed;
using SimulatedAnnealing.Server.Models.DTOs;

namespace SimulatedAnnealing.Server.Services.Database;

public interface IDbRepository
{
    Task<GerrymanderingResult?> GetLocalResults(LocalResultsQuery request);
    Task<Voivodeship?> GetVoivodeship(ConfigurationQuery request);
}
