using SimulatedAnnealing.Server.Models.DTOs;

namespace SimulatedAnnealing.Server.Models.Requests;

public class OptimizeParliamentDistrictRequest
{
    public int Year { get; set; }
    public string PoliticalParty { get; set; }
    public string VoivodeshipName { get; set; }

}

public static class Extensions
{
    public static LocalResultsRequest ToLocalResutlsRequest(this OptimizeParliamentDistrictRequest request)
    {
        return new LocalResultsRequest
        {
            VoivodeshipName = request.VoivodeshipName,
            Year = request.Year,
            PoliticalParty = request.PoliticalParty,
        };
    }
}
