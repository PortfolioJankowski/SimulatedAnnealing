using SimulatedAnnealing.Server.Models.DTOs;

namespace SimulatedAnnealing.Server.IntegrationTests;
public class DbRepositoryTests : BaseIntegrationTest
{
    public DbRepositoryTests(DatabaseEndpointFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetVoivodeShip_ShouldReturnValidVoivodeship_WhenProperRequestProvided()
    {
        var request = new InitialStateRequest()
        {
            VoivodeshipName = "małopolskie",
            Year = 2024
        };

        var result = await _dbRepository.GetVoivodeshipAsync(request);
        Assert.NotNull(result);

    }
}

