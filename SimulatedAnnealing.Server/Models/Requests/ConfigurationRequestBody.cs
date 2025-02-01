using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace SimulatedAnnealing.Server.Models.DTOs;

public class ConfigurationRequestBody
{
    public string VoivodeshipName { get; set; }
    public int Year { get; set; }
}

