using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimulatedAnnealing.Server.Models.Algorithm.Fixed;
using SimulatedAnnealing.Server.Models.DTOs;
using SimulatedAnnealing.Server.Services.Database;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SimulatedAnnealing.Server.Controllers;

public static class DatabaseEndpoints
{
    public static void MapDatabaseEndpoints(this IEndpointRouteBuilder app) //Extension to execute in Program.cs
    {
                                                    //Delegate
        app.MapPost("api/Database/GetLocalResults", GetLocalResults).RequireAuthorization();
        app.MapPost("api/Database/GetInitialState", GetInitialState).RequireAuthorization();
    }

    public static async Task<IResult> GetInitialState([FromBody] ConfigurationQuery request, IDbRepository dbRepository)
    {
        var initialVoivodeship = await dbRepository.GetVoivodeship(request);

        var jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            WriteIndented = true
        };

        return initialVoivodeship != null ? Results.Json(initialVoivodeship, jsonOptions) : Results.NotFound();
    }

    public static async Task<IResult> GetLocalResults([FromBody] LocalResultsQuery request, IDbRepository dbRepository) //DI into Method
    {
        var localResults = await dbRepository.GetLocalResults(request);
        return localResults != null ? Results.Json(localResults) : Results.NoContent();
    }
    
}

