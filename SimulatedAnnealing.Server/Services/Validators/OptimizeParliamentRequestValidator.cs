using FluentValidation;
using SimulatedAnnealing.Server.Models.Enums;
using SimulatedAnnealing.Server.Models.Requests;

namespace SimulatedAnnealing.Server.Services.Validators;

public class OptimizeParliamentRequestValidator : AbstractValidator<OptimizeParliamentSeatsRequest>
{
    private int[] _availableYears = new int[2] { 2023, 2019 };
    public OptimizeParliamentRequestValidator()
    {
        RuleFor(x => x.year)
            .Must(y => _availableYears.Contains(y))
            .WithMessage("Invalid year provided!");

        RuleFor(x => x.comittee)
            .Must(c => Enum.IsDefined(typeof(PoliticalCommittee), c))
            .WithMessage("Provide valid comittee!");
    }
}
