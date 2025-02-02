using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace SimulatedAnnealing.Server.Models.DTOs;

public class InitialStateRequest
{
    public string VoivodeshipName { get; set; }
    public int Year { get; set; }
}

