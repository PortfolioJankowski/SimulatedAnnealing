using SimulatedAnnealing.Server.Models.Variable;

namespace SimulatedAnnealing.Server.Services.Behavioral;
public class SimulatedAnnealingService
{
    private readonly ComplianceService _complianceService;
    public SimulatedAnnealingService(ComplianceService complianceService)
    {
        _complianceService = complianceService;
    }
    public State Optimize()
    {

        return new State(_complianceService);
    }
}
