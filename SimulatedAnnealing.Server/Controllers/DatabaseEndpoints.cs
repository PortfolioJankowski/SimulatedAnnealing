using Microsoft.AspNetCore.Authorization;
using SimulatedAnnealing.Server.Models.DTOs;
using SimulatedAnnealing.Server.Services.Database;

namespace SimulatedAnnealing.Server.Controllers;

public static class DatabaseEndpoints
{
    public static void MapDatabaseEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/Database/GetLocalResults", GetLocalResults).RequireAuthorization();
    }
    public static async Task<IResult> GetLocalResults(GetLocalResultsRequest request, IDbRepository dbRepository)
    {
        var localResults = await dbRepository.GetLocalResults(request);
        return localResults != null ? Results.Json(localResults) : Results.NoContent();
    }
}

