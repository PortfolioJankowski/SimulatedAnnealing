using SimulatedAnnealing.Server.Models.Algorithm.Fixed;
using SimulatedAnnealing.Server.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SimulatedAnnealing.Server.Services.Authentication;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace SimulatedAnnealing.Server.IntegrationTests;

public class DatabaseEndpointsTests : IClassFixture<DatabaseEndpointFactory>
{
    private readonly HttpClient _client;
    private readonly DatabaseEndpointFactory _factory;
    public DatabaseEndpointsTests(DatabaseEndpointFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    //[Fact]
    //public async Task GetInitialState_ReturnsExpectedVoivodeship_WhenDataExists()
    //{
    //    // Arrange
    //    var request = new ConfigurationRequestBody { Year = 2024, VoivodeshipName = "małopolskie" };
    //    var token = await GetJwtTokenForTestUser();
    //    _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
    //    // Act
    //    var response = await _client.PostAsJsonAsync("api/Database/GetInitialState", request);

    //    // Assert
    //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    //    var result = await response.Content.ReadFromJsonAsync<Voivodeship>();
    //    Assert.NotNull(result);
    //    Assert.Equal("małopolskie", result.Name);
    //}
    //private async Task<string> GetJwtTokenForTestUser()
    //{
    //    var userService = await _factory.GetScopedService<IUserService>();
    //    var (testUser, ex)  = await userService.TryLoginUserAsync(new Models.Authentication.Dto.LoginDto()
    //    {
    //        Username = "testuUer",
    //        Password = "testPassword1"
    //    });

    //    return testUser!.Token;
    //}

    
}

