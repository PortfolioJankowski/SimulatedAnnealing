using System.ComponentModel.DataAnnotations;

namespace SimulatedAnnealing.Server.Models.DTOs;

public class ConfigurationQuery
{
    [Required]
    public string VoivodeshipName { get; set; }
    [Required]
    public int Year { get; set; }
}

