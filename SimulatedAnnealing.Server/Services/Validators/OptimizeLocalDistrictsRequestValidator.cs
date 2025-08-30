using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using SimulatedAnnealing.Server.Models.Algorithm;
using SimulatedAnnealing.Server.Models.DTOs;
using SimulatedAnnealing.Server.Models.Requests;

namespace SimulatedAnnealing.Server.Services.Validators
{
    public class OptimizeLocalDistrictsRequestValidator : AbstractValidator<OptimizeLocalDistrictsRequest>
    {
        public OptimizeLocalDistrictsRequestValidator(IOptions<AvailableDistricsOptions> availableDistricts)
        {
            RuleFor(x => x.DistrictInformation)
                .Must(d => IsValidDistrict(availableDistricts, d))
                .WithMessage("Invalid district information provided.");
        }

        private bool IsValidDistrict(IOptions<AvailableDistricsOptions> availableDistricts, LocalResultsRequest districtInformation)
        {
            if (districtInformation == null)
                return false;

            if (!availableDistricts.Value.Districts.TryGetValue(districtInformation.VoivodeshipName, out var districtInfo))
                return false;

            if (!districtInfo.TryGetValue(districtInformation.Year.ToString(), out var politicalParties))
                return false;

            return politicalParties.Contains(districtInformation.PoliticalParty);
        }
    }
}
