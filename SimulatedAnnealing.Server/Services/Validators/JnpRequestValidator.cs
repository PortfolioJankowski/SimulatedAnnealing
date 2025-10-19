using FluentValidation;
using SimulatedAnnealing.Server.Models.JNP;

namespace SimulatedAnnealing.Server.Services.Validators;

public class JnpRequestValidator : AbstractValidator<CheckJnpRequest>
{
    public JnpRequestValidator()
    {
        var avalableYears = new List<int> { 2023 };
        RuleFor(x => x.Year)
            .NotEmpty().WithMessage("Year is required.")
            .Must(year => avalableYears.Contains(year))
            .WithMessage("Invalid Year");
    }
}