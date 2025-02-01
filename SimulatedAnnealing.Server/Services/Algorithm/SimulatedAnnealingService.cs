using SimulatedAnnealing.Server.Models.Algorithm.Variable;

namespace SimulatedAnnealing.Server.Services.Behavioral;
public class SimulatedAnnealingService
{
    private readonly ComplianceService _complianceService;
    public SimulatedAnnealingService(ComplianceService complianceService)
    {
        _complianceService = complianceService;
    }
    public DistrictState Optimize()
    {

        return new DistrictState(_complianceService);
    }
}
