using System.ComponentModel.DataAnnotations;

namespace SimulatedAnnealing.Server.Models.DTOs;

public class LocalResultsQuery
{
    [Required]
    [Range(2014,2024)]
    public int Year { get; set; }
    [Required]
    public string PoliticalParty { get; set; }
    [Required]
    public string Voivodeship { get; set; }
}

