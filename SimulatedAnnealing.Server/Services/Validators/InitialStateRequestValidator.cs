using FluentValidation;
using Microsoft.Extensions.Options;
using SimulatedAnnealing.Server.Models.Algorithm;
using SimulatedAnnealing.Server.Models.DTOs;

namespace SimulatedAnnealing.Server.Services.Validators;

public class InitialStateRequestValidator : AbstractValidator<InitialStateRequest>
{
    public InitialStateRequestValidator(IOptions<AvailableDirstricsOptions> availableDistricts)
    {
        var districts = availableDistricts.Value.Districts;

        RuleFor(x => x.VoivodeshipName)
            .NotEmpty().WithMessage("Voivodeship name is required.")
            .Must(name => districts.ContainsKey(name.ToLower()))
            .WithMessage("Invalid Voivodeship name");


        RuleFor(x => x.Year)
           .Must((request, year) => 
            districts.TryGetValue(request.VoivodeshipName.ToLower(), out var validYears) && validYears.Keys.Contains(year.ToString()))
           .WithMessage("The selected year is not available for this Voivodeship.");
    }
}
