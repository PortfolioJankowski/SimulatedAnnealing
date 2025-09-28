using SimulatedAnnealing.Server.Models.Algorithm.Variable;
using SimulatedAnnealing.Server.Models.Configuration;
using SimulatedAnnealing.Server.Models.Requests;

namespace SimulatedAnnealing.Server.Services.Algorithm;

public static class IndicatorService
{
    public static Indicator SetNewIndicator(VoivodeshipState state, OptimizeLocalDistrictsRequest request, AlgorithmConfiguration config, bool isParliament = false)
    {
        var districtConfig = request.DistrictInformation;

        if (state.ActualConfiguration == null)
        {
            return new Indicator { Seats = 0, Score = 0 };
        }

        return new Indicator()
        {
            Seats = isParliament ?
                GetParliamentSeatsSum(state, districtConfig.PoliticalParty)
                : GetLocalSeatsSum(state, districtConfig.PoliticalParty),
            Score = isParliament ?
                GetParliamentSeatsSum(state, districtConfig.PoliticalParty) + GetParliamentGerrymanderingScore(state, request, config)
                : GetLocalSeatsSum(state, districtConfig.PoliticalParty) + GetLocalGerrymanderingScore(state, request, config)
        };
    }

    private static int GetLocalSeatsSum(VoivodeshipState state, string choosenParty)
    {
        return state.DistrictVotingResults!.Values
            .SelectMany(district => district)
            .Where(wyniki => wyniki.Key == choosenParty)
            .Sum(mandaty => mandaty.Value);
    }

    /// <summary>
    /// Oblicza Współczynnik Gerrymanderingu (Score) dla wyborów lokalnych.
    /// Wynik jest sumą efektów "pakowania" (packing) i "rozbijania" (cracking)
    /// z każdego pojedynczego okręgu wchodzącego w skład województwa.
    /// Wyższy Score oznacza bardziej pożądaną (lepszą) konfigurację okręgów dla wybranej partii.
    /// </summary>
    /// <param name="state">Aktualny stan konfiguracji okręgów w województwie.</param>
    /// <param name="request">Informacje o wyborach i partii docelowej.</param>
    /// <param name="config">Parametry algorytmu, w tym progi (thresholds) i wagi (weights) dla efektów packing/cracking.</param>
    /// <returns>Łączny Współczynnik Skuteczności (Score) za gerrymandering dla całego województwa.</returns>
    private static double GetLocalGerrymanderingScore(VoivodeshipState state, OptimizeLocalDistrictsRequest request, AlgorithmConfiguration config)
    {
        double totalGerrymanderingScore = 0;

        double packingThreshold = config.PackingThreshold;
        double crackingThreshold = config.CrackingThreshold;
        double packingWeight = config.PackingWeight;
        double crackingWeight = config.CrackingWeight;

        foreach (var district in state.ActualConfiguration!.Districts)
        {
            double totalVotesForChosenParty = 0;
            double totalVotes = 0;

            foreach (var county in district.Counties)
            {
                var countyResults = county.VotingResults.Where(w => w.Year == request.DistrictInformation.Year).ToList();

                if (countyResults.Any())
                {
                    int countyVotes = countyResults.Sum(r => r.NumberVotes ?? 0);

                    var choosenPartyResult = countyResults
                        .Where(r => r.Committee == request.DistrictInformation.PoliticalParty)
                        .FirstOrDefault();

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

                double packingEffect = System.Math.Max(0, targetPartyPercentage - packingThreshold);
                double crackingEffect = System.Math.Max(0, crackingThreshold - targetPartyPercentage);

                totalGerrymanderingScore += (packingWeight * packingEffect) + (crackingWeight * crackingEffect);
            }
        }

        return totalGerrymanderingScore;
    }


    //======================= PARLAMENT 

    private static int GetParliamentSeatsSum(VoivodeshipState state, string choosenParty)
    {
        return state.ParliamentDistrictVotingResults!.Values
            .SelectMany(district => district)
            .Where(wyniki => wyniki.Key == choosenParty)
            .Sum(mandaty => mandaty.Value);
    }

    private static double GetParliamentGerrymanderingScore(VoivodeshipState state, OptimizeLocalDistrictsRequest request, AlgorithmConfiguration config)
    {
        double totalGerrymanderingScore = 0; 

        double packingThreshold = config.PackingThreshold;
        double crackingThreshold = config.CrackingThreshold;
        double packingWeight = config.PackingWeight;
        double crackingWeight = config.CrackingWeight;

        foreach (var district in state.ActualConfiguration!.ParliamentDistricts)
        {
            double totalVotesForChosenParty = 0;
            double totalVotes = 0;

            foreach (var county in district.TerytCounties)
            {
                var countyResults = county.ParliamentVotingResults.Where(w => w.Year == request.DistrictInformation.Year).ToList();

                if (countyResults.Any())
                {
                    int countyVotes = countyResults.Sum(r => r.NumberVotes);

                    var choosenPartyResult = countyResults
                        .Where(r => r.Comitee == request.DistrictInformation.PoliticalParty) 
                        .FirstOrDefault();

                    if (choosenPartyResult != null)
                    {
                        totalVotesForChosenParty += choosenPartyResult.NumberVotes;
                    }
                    totalVotes += countyVotes;
                }
            }

            if (totalVotes > 0)
            {
                double targetPartyPercentage = totalVotesForChosenParty / totalVotes;

                double packingEffect = System.Math.Max(0, targetPartyPercentage - packingThreshold);
                double crackingEffect = System.Math.Max(0, crackingThreshold - targetPartyPercentage);

                totalGerrymanderingScore += (packingWeight * packingEffect) + (crackingWeight * crackingEffect);
            }
        }

        return totalGerrymanderingScore;
    }
}