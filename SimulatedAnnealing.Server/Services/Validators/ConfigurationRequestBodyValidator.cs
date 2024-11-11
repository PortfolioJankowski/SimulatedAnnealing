using FluentValidation;
using SimulatedAnnealing.Server.Models.DTOs;

namespace SimulatedAnnealing.Server.Services.Validators;

public class ConfigurationRequestBodyValidator : AbstractValidator<ConfigurationRequestBody>
{
    public ConfigurationRequestBodyValidator()
    {
        RuleFor(x => x.VoivodeshipName)
            .NotEmpty().WithMessage("Voivodeship name is required.");

        RuleFor(x => x.Year)
           .InclusiveBetween(2014, 2024).WithMessage("Year must be between 2014 and 2024.");
    }
}
