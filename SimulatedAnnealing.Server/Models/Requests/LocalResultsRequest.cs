using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace SimulatedAnnealing.Server.Models.DTOs;

public class LocalResultsRequest
{
    public int Year { get; set; }
    public string PoliticalParty { get; set; }
    public string VoivodeshipName { get; set; }
}
