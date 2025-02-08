using SimulatedAnnealing.Server.Models.Algorithm.Variable;
using SimulatedAnnealing.Server.Models.Requests;

namespace SimulatedAnnealing.Server.Services.Behavioral;
public class SimulatedAnnealingService
{
    private readonly ComplianceService _complianceService;
    public SimulatedAnnealingService(ComplianceService complianceService)
    {
        _complianceService = complianceService;
    }
    public DistrictState Optimize(OptimizeLocalDistrictsRequest request)
    {

        return new DistrictState(_complianceService);
    }
}
