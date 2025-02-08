using Azure.Core;
using FluentValidation;
using Microsoft.Extensions.Options;
using SimulatedAnnealing.Server.Models.Algorithm;
using SimulatedAnnealing.Server.Models.Algorithm.Fixed;
using SimulatedAnnealing.Server.Models.DTOs;

namespace SimulatedAnnealing.Server.Services.Validators;

public class LocalResultsRequestValidator : AbstractValidator<LocalResultsRequest>
{
    public LocalResultsRequestValidator(IOptions<AvailableDirstricsOptions> availableDistricts)
    {
        var districts = availableDistricts.Value;

        RuleFor(x => x.Year)
            .Must((request, year) => IsValidRequest(request, districts))
            .WithMessage("Invalid voivodeship or year provided.");
    }

    private bool IsValidRequest(LocalResultsRequest request,  AvailableDirstricsOptions districts)
    {
        if (!districts.Districts.TryGetValue(request.VoivodeshipName.ToLower(), out var availableYears))
            return false; 

        if (!availableYears.TryGetValue(request.Year.ToString(), out var availableParties))
            return false; 

        return availableParties.Contains(request.PoliticalParty);
    }
}
