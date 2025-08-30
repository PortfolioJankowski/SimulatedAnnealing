using SimulatedAnnealing.Server.Models.Algorithm.Variable;
using SimulatedAnnealing.Server.Models.Requests;

namespace SimulatedAnnealing.Server.Services.Algorithm;

public static class IndicatorService
{
    public static Indicator SetNewIndicator(VoivodeshipState state, OptimizeLocalDistrictsRequest request, AlgorithmConfiguration config)
    {
        var districtConfig = request.DistrictInformation;
        return new Indicator()
        {
            Seats = GetSeatsSum(state, districtConfig.PoliticalParty),
            Score = GetSeatsSum(state, districtConfig.PoliticalParty) + GetGerrymanderingScore(state, request, config),
        };
    }
    private static int GetSeatsSum(VoivodeshipState state, string choosenParty)
    {
        return state.DistrictVotingResults!.Values
            .SelectMany(district => district)
            .Where(wyniki => wyniki.Key == choosenParty)
            .Sum(mandaty => mandaty.Value);
    }

    private static double GetGerrymanderingScore(VoivodeshipState state, OptimizeLocalDistrictsRequest request, AlgorithmConfiguration config)
    {
        double packingEffect = 0;
        double crackingEffect = 0;

        double packingThreshold = config.PackingThreshold;
        double crackingThreshold = config.CrackingThreshold;
        double packingWeight = config.PackingWeight;
        double crackingWeight = config.CrackingWeight;

        double totalVotesForChosenParty = 0; 
        double totalVotes = 0; 

        foreach (var district in state.ActualConfiguration!.Districts)
        {
            foreach (var county in district.Counties)
            {
                var countyResults = county.VotingResults.Where(w => w.Year == request.DistrictInformation.Year).ToList();

                if (countyResults.Any())
                {
                    int countyVotes = countyResults.Sum(r => r.NumberVotes ?? 0); // Liczba głosów w powiecie
                    var choosenPartyResult = countyResults.Where(r => r.Committee== request.DistrictInformation.PoliticalParty).FirstOrDefault();
                    if (choosenPartyResult != null)
                    {
                        totalVotesForChosenParty += choosenPartyResult.NumberVotes ?? 0;
                    }
                    totalVotes += countyVotes;
                }
            }

            if (totalVotes > 0)
            {
                double targetPartyPercentage = totalVotesForChosenParty / totalVotes;

                // efekt pakowania rośnie, kiedy pakowanie okręgu jest większe niż pakowanie tresholdu
                packingEffect += System.Math.Max(0, targetPartyPercentage - packingThreshold);
                crackingEffect += System.Math.Max(0, crackingThreshold - targetPartyPercentage);
            }
        }

        // Calculate total effect
        return (packingWeight * packingEffect) + (crackingWeight * crackingEffect);
    }
}