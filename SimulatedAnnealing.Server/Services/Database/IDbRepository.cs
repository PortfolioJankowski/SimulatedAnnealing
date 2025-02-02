using Microsoft.Extensions.Caching.Distributed;
using SimulatedAnnealing.Server.Models.Algorithm.Fixed;
using SimulatedAnnealing.Server.Models.DTOs;

namespace SimulatedAnnealing.Server.Services.Database;

public interface IDbRepository
{
    Task<Voivodeship?> GetVoivodeshipAsync(InitialStateRequest request);
    Task<GerrymanderingResult?> GetGerrymanderringResultsAsync(LocalResultsRequestBody request);
}
