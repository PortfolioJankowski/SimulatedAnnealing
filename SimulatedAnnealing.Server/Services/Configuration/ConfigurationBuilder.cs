using Docker.DotNet.Models;
using Microsoft.EntityFrameworkCore;
using SimulatedAnnealing.Server.Models.Configuration;
using SimulatedAnnealing.Server.Models.DTOs;
using SimulatedAnnealing.Server.Services.Database;

namespace SimulatedAnnealing.Server.Services.Configuration;

public class AlgorithmConfigurationBuilder
{
    private readonly PhdApiContext _context;

    public AlgorithmConfigurationBuilder(PhdApiContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Buduje konfigurację algorytmu symulowanego wyżarzania dla danego województwa i partii.
    /// Łączy podejście strategiczne (pozycja partii w systemie politycznym) z podejściem statystycznym (kształt rozkładu głosów).
    /// 1. Klasyfikuje partię jako dużą, średnią, małą lub bliską progu i nadaje wagi bazowe.
    /// 2. Analizuje rozkład głosów w województwie (średnia, wariancja, odchylenie, skośność, kurtoza).
    /// 3. Dostraja parametry (Temperature, CoolingRate, StepSize, Packing/CrackingThresholds) pod charakterystykę regionu.
    /// 4. Zwraca gotową konfigurację dopasowaną do danej partii i województwa.
    /// </summary>
    public AlgorithmConfiguration Build(LocalResultsRequest request, bool isParliament = false)
    {
        AlgorithmConfiguration config;
        if (isParliament)
        {
            config = HandleParliamentCase(request);
            return config;
        }

        string partia = request.PoliticalParty;
        string wojewodztwo = request.VoivodeshipName;
        int rok = request.Year;

        // 1. Pobranie województwa i powiatów
        var woj = _context.Voivodeships
            .Include(v => v.Districts)
            .ThenInclude(d => d.Counties)
            .ThenInclude(c => c.VotingResults)
            .FirstOrDefault(w => w.Name == wojewodztwo);

        var powiatyId = woj!.Districts.SelectMany(d => d.Counties).Select(c => c.CountyId).ToList();

        // 2. Pobranie wyników wyborów
        var results = _context.VotingResults
            .Where(r => r.Year == rok && powiatyId.Contains((int)r.CountyId))
            .ToList();

        double totalVotes = results.Sum(r => r.NumberVotes ?? 0);

        var partyPercentages = results
            .GroupBy(r => r.Committee)
            .ToDictionary(
                g => g.Key,
                g => (g.Sum(x => x.NumberVotes ?? 0)) / totalVotes
            );

        double chosenPartyPercentage = partyPercentages.ContainsKey(partia) ? partyPercentages[partia] : 0.0;
        var values = partyPercentages.Values.ToList();

        // 3. Statystyki
        double mean = values.Average();
        double variance = values.Sum(v => Math.Pow(v - mean, 2)) / values.Count;
        double stdDev = Math.Sqrt(variance);
        double skewness = values.Sum(v => Math.Pow((v - mean) / stdDev, 3)) / values.Count;
        double kurtosis = values.Sum(v => Math.Pow((v - mean) / stdDev, 4)) / values.Count - 3;

        // 4. Strategiczne bazowe parametry
        double packingWeight, crackingWeight;
        double packingThreshold, crackingThreshold;

        if (chosenPartyPercentage >= 0.35)
        {
            // Duża partia
            packingWeight = 1.0;
            crackingWeight = 1.8;
        }
        else if (chosenPartyPercentage >= 0.15)
        {
            // Średnia partia
            packingWeight = 1.2;
            crackingWeight = 1.2;
        }
        else
        {
            // Mała partia
            packingWeight = 1.8;
            crackingWeight = 1.0;
        }

        // Partia bliska progu wyborczego (5%) → wzmocnienie
        if (chosenPartyPercentage is >= 0.04 and <= 0.07)
        {
            packingWeight += 0.5;
            crackingWeight += 0.5;
        }

        packingThreshold = chosenPartyPercentage + 0.05;
        crackingThreshold = chosenPartyPercentage - 0.05;

        // 5. Adaptacyjne dostrajanie na podstawie statystyk
        double temperature = 100 * (1 + Math.Abs(skewness));
        double coolingRate = 0.995 - Math.Min(0.05, kurtosis / 10);
        double stepSize = 1 + stdDev * 10;

        packingWeight *= (1 + stdDev);
        crackingWeight *= (1 + Math.Max(0, skewness));

        // 6. Złożenie konfiguracji
        config = new AlgorithmConfiguration
        {
            Temperature = temperature,
            CoolingRate = coolingRate,
            StepSize = stepSize,
            MaxIterations = 5000 + (int)(stdDev * 2000),
            PackingThreshold = packingThreshold,
            CrackingThreshold = crackingThreshold,
            PackingWeight = packingWeight,
            CrackingWeight = crackingWeight
        };

       
        return config;
    }

    private AlgorithmConfiguration HandleParliamentCase(LocalResultsRequest request)
    {
        string partia = request.PoliticalParty;
        string wojewodztwo = request.VoivodeshipName;
        int rok = request.Year;

        // 1. Pobranie województwa i powiatów
        var woj = _context.Voivodeships
            .Include(v => v.ParliamentDistricts)
            .ThenInclude(d => d.TerytCounties)
            .ThenInclude(c => c.ParliamentVotingResults)
            .FirstOrDefault(w => w.Name == wojewodztwo);
        

        var powiatyId = woj!.ParliamentDistricts.SelectMany(d => d.TerytCounties).Select(c => c.Teryt).ToList();

        // 2. Pobranie wyników wyborów
        var results = _context.ParliamentVotingResults
            .Where(r => r.Year == rok && powiatyId.Contains(r.CountyTeryt))
            .ToList();

        double totalVotes = results.Sum(r => r.NumberVotes);

        var partyPercentages = results
            .GroupBy(r => r.Comitee)
            .ToDictionary(
                g => g.Key,
                g => (g.Sum(x => x.NumberVotes)) / totalVotes
            );

        double chosenPartyPercentage = partyPercentages.ContainsKey(partia) ? partyPercentages[partia] : 0.0;
        var values = partyPercentages.Values.ToList();

        // 3. Statystyki
        double mean = values.Average();
        double variance = values.Sum(v => Math.Pow(v - mean, 2)) / values.Count;
        double stdDev = Math.Sqrt(variance);
        double skewness = values.Sum(v => Math.Pow((v - mean) / stdDev, 3)) / values.Count;
        double kurtosis = values.Sum(v => Math.Pow((v - mean) / stdDev, 4)) / values.Count - 3;

        // 4. Strategiczne bazowe parametry
        double packingWeight, crackingWeight;
        double packingThreshold, crackingThreshold;

        if (chosenPartyPercentage >= 0.35)
        {
            // Duża partia
            packingWeight = 1.0;
            crackingWeight = 1.8;
        }
        else if (chosenPartyPercentage >= 0.15)
        {
            // Średnia partia
            packingWeight = 1.2;
            crackingWeight = 1.2;
        }
        else
        {
            // Mała partia
            packingWeight = 1.8;
            crackingWeight = 1.0;
        }

        // Partia bliska progu wyborczego (5%) → wzmocnienie
        if (chosenPartyPercentage is >= 0.04 and <= 0.07)
        {
            packingWeight += 0.5;
            crackingWeight += 0.5;
        }

        packingThreshold = chosenPartyPercentage + 0.05;
        crackingThreshold = chosenPartyPercentage - 0.05;

        // 5. Adaptacyjne dostrajanie na podstawie statystyk
        double temperature = 100 * (1 + Math.Abs(skewness));
        double coolingRate = 0.995 - Math.Min(0.05, kurtosis / 10);
        double stepSize = 1 + stdDev * 10;

        packingWeight *= (1 + stdDev);
        crackingWeight *= (1 + Math.Max(0, skewness));

        // 6. Złożenie konfiguracji
        return new AlgorithmConfiguration
        {
            Temperature = temperature,
            CoolingRate = coolingRate,
            StepSize = stepSize,
            MaxIterations = 5000 + (int)(stdDev * 2000),
            PackingThreshold = packingThreshold,
            CrackingThreshold = crackingThreshold,
            PackingWeight = packingWeight,
            CrackingWeight = crackingWeight
        };
    }
}

//Temperature ≈ 255,7 – bardzo wysoka temperatura początkowa → sugeruje dużą zmienność w rozkładzie głosów(wysoka skośność). Algorytm zaczyna szeroko eksplorować, bo istnieje potencjał do znalezienia bardzo różnych konfiguracji.
//CoolingRate = 0,945 – relatywnie niskie tempo chłodzenia → algorytm będzie długo testował alternatywne układy, co ma sens przy dużej niepewności (niestały rozkład poparcia).
//StepSize ≈ 2,36 – krok nie jest minimalny, co wskazuje na wyraźne odchylenie standardowe w poparciu partii w powiatach.Partia ma nierównomierne wyniki w regionie.
//MaxIterations = 5271 – liczba iteracji umiarkowana, ale dostosowana do tej wariancji.
//PackingThreshold ≈ 0,489 / CrackingThreshold ≈ 0,389 – progi są ustawione wokół 39–49%. To oznacza, że partia w tym województwie zdobywa ok. 40–45% głosów.Czyli jest dużą partią.
//PackingWeight ≈ 1,14 / CrackingWeight ≈ 4,6 – algorytm zdecydowanie bardziej premiuje „cracking” (rozbijanie poparcia przeciwników) niż „packing”. To logiczne, bo partia ma już dużą bazę głosów i bardziej opłaca się rozproszyć konkurentów niż pakować swoich w kilku mocnych okręgach.

//🔎 Wnioski:

//To jest partia dominująca w województwie(ok. 40–45%).
//Ma silne, ale nierównomierne poparcie(wysoka temperatura i step size).
//Strategia optymalizacji skupia się na rozbijaniu przeciwników(wysoki cracking weight).
//Algorytm przewiduje, że największe zyski są nie w pakowaniu swoich wyborców, ale w „wykradaniu” mandatów partiom, które lokalnie mają po kilkanaście–kilkadziesiąt procent.