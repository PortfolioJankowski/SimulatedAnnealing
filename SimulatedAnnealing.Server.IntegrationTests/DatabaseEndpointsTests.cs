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
    private readonly ConfigurationRequestBody _configurationRequestBody;
    private static string _token;
    public DatabaseEndpointsTests(DatabaseEndpointFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _configurationRequestBody = new ConfigurationRequestBody()
        {
            VoivodeshipName = "małopolskie",
            Year = 2024
        };

        if(string.IsNullOrEmpty(_token))
        {
            _token = GetJwtTokenForTestUser().Result;
        }
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
    }

    [Fact]
    public async Task GetInitialState_ReturnsExpectedVoivodeship_WhenDataExists()
    { 
        // Act
        var response = await _client.PostAsJsonAsync("api/Database/GetInitialState", _configurationRequestBody);
        var result = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetInitialState_ReturnsBadRequest_WhenRequestIsEmpty()
    {
        // Act
        var response = await _client.PostAsJsonAsync("api/Database/GetInitialState", new ConfigurationRequestBody());

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetInitialState_ReturnsNotFound_WhenRequestIsInvalid()
    {
        // Arrange
        _configurationRequestBody.VoivodeshipName = "mauopolskie";

        // Act
        var response = await _client.PostAsJsonAsync("api/Database/GetInitialState", _configurationRequestBody);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<string> GetJwtTokenForTestUser()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            var (testUser, ex) = await userService.TryLoginUserAsync(new Models.Authentication.Dto.LoginDto()
            {
                Username = "testUser",
                Password = "testPassword1!"
            });

            return testUser!.Token;
        }
    }



}

