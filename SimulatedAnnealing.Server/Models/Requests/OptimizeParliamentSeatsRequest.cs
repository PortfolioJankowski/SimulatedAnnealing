using SimulatedAnnealing.Server.Models.Enums;

namespace SimulatedAnnealing.Server.Models.Requests;

public record OptimizeParliamentSeatsRequest(int year, PoliticalCommittee comittee);
