using System.ComponentModel.DataAnnotations;

namespace SimulatedAnnealing.Server.Models.Authentication.Dto;
public class LoginDto
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }

}

