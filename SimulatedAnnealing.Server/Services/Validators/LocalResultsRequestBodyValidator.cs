using FluentValidation;
using SimulatedAnnealing.Server.Models.DTOs;

namespace SimulatedAnnealing.Server.Services.Validators;

public class LocalResultsRequestBodyValidator : AbstractValidator<LocalResultsRequestBody>
{
    public LocalResultsRequestBodyValidator()
    {
        RuleFor(x => x.Year)
            .NotEmpty().WithMessage("Year is required.")
            .InclusiveBetween(2014, 2024).WithMessage("Year must be between 2014 and 2024.");

        RuleFor(x => x.PoliticalParty)
            .NotEmpty().WithMessage("Political party is required.");

        RuleFor(x => x.Voivodeship)
            .NotEmpty().WithMessage("Voivodeship is required.");
    }
}
