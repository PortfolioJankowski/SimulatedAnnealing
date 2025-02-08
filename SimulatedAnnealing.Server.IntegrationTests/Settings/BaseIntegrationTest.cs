using Microsoft.Extensions.DependencyInjection;
using SimulatedAnnealing.Server.Models.DTOs;
using SimulatedAnnealing.Server.Services.Authentication;
using SimulatedAnnealing.Server.Services.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedAnnealing.Server.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<DatabaseEndpointFactory> //shared resources for a tests
{
    private readonly IServiceScope _scope;
    protected readonly IDbRepository _dbRepository; //particular test class will have access to this one
    protected readonly IUserService _userService;
    protected readonly InitialStateRequest _configurationRequestBody;
    protected readonly LocalResultsRequest _localResultsRequestBody;  

    protected BaseIntegrationTest(DatabaseEndpointFactory factory)
    {
        _scope = factory.Services.CreateScope(); //creates a new scope for each tests, ensuring each test gets its own set of dependencies
        _dbRepository = _scope.ServiceProvider.GetRequiredService<IDbRepository>();
        _userService = _scope.ServiceProvider.GetRequiredService<IUserService>();   

        _localResultsRequestBody = new LocalResultsRequest()
        {
            PoliticalParty = "KOMITET WYBORCZY PRAWO I SPRAWIEDLIWOŚĆ",
            Year = 2024,
            VoivodeshipName = "małopolskie"
        };

        _configurationRequestBody = new InitialStateRequest()
        {
            Year = 2024,
            VoivodeshipName = "małopolskie"
        };

    }
}

