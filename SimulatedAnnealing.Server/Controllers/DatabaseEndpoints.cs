using Microsoft.AspNetCore.Mvc;
using SimulatedAnnealing.Server.Models.DTOs;
using SimulatedAnnealing.Server.Services.Database;
using FluentValidation;
using Microsoft.Extensions.Caching.Distributed;

namespace SimulatedAnnealing.Server.Controllers;

public static class DatabaseEndpoints
{
    public static void MapDatabaseEndpoints(this IEndpointRouteBuilder app) //Extension to execute in Program.cs
    {
                                                    //Delegate
        app.MapPost("api/Database/GetLocalResults", GetLocalResults).RequireAuthorization();
        app.MapPost("api/Database/GetInitialState", GetInitialState).RequireAuthorization();
    }

    public static async Task<IResult> GetLocalResults([FromBody] LocalResultsRequestBody request, IDbRepository dbRepository, IValidator<LocalResultsRequestBody> validator) //DI into Method
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Results.BadRequest(validationResult.Errors);

        var localResults = await dbRepository.GetGerrymanderringResultsAsync(request);

        if (localResults == null)
            return Results.NoContent();

        return Results.Json(localResults);
    }

    public static async Task<IResult> GetInitialState([FromBody] InitialStateRequest request, IDbRepository dbRepository, IValidator<InitialStateRequest> validator) //Inside DTO files i inserted additional validator classes
    {
        var validationResult = await validator.ValidateAsync(request); //Added sth similar to ModelState.Valid -> minimal api doesnt support that mechanism by default
        if (!validationResult.IsValid)
            return Results.BadRequest(validationResult.Errors);

        var initialVoivodeship = await dbRepository.GetVoivodeshipAsync(request); //Avoided one-liner return statement -> imo readabillity improved
        if (initialVoivodeship == null)
            return Results.NotFound();

        return Results.Json(initialVoivodeship);
    }
}

