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
        public OptimizeLocalDistrictsRequestValidator(IOptions<AvailableDirstricsOptions> availableDistricts)
        {
            RuleFor(x => x.DistrictInformation)
                .Must(d => IsValidDistrict(availableDistricts, d))
                .WithMessage("Invalid district information provided.");

            RuleFor(x => x.AlgorithmConfiguration)
                .Must(config => IsValidAlgorithmConfiguration(config))
                .WithMessage("Invalid algorithm configuration.");
        }

        private bool IsValidDistrict(IOptions<AvailableDirstricsOptions> availableDistricts, LocalResultsRequest districtInformation)
        {
            if (districtInformation == null)
                return false;

            if (!availableDistricts.Value.Districts.TryGetValue(districtInformation.VoivodeshipName, out var districtInfo))
                return false;

            if (!districtInfo.TryGetValue(districtInformation.Year.ToString(), out var politicalParties))
                return false;

            return politicalParties.Contains(districtInformation.PoliticalParty);
        }

        private bool IsValidAlgorithmConfiguration(AlgorithmConfiguration algorithmConfiguration)
        {
            if (algorithmConfiguration == null)
                return false;

            bool areThresholdsPositive = algorithmConfiguration.PackingThreshold >= 0 && algorithmConfiguration.CrackingThreshold >= 0;
            bool areOtherValuesPositive = algorithmConfiguration.Temperature > 0 &&
                                          algorithmConfiguration.MaxIterations > 0 &&
                                          algorithmConfiguration.CoolingRate > 0 &&
                                          algorithmConfiguration.StepSize > 0;

            return areThresholdsPositive && areOtherValuesPositive;
        }
    }
}
