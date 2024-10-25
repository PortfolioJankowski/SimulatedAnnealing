using Microsoft.AspNetCore.Identity;

namespace SimulatedAnnealing.Server.Models.Authentication.Exceptions
{
    public class UserCreationException : Exception
    {
        public IEnumerable<IdentityError> Errors { get; }

        public UserCreationException(string message, IEnumerable<IdentityError> errors) : base(message)
        {
            Errors = errors;
        }
    }
}
