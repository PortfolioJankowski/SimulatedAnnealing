namespace SimulatedAnnealing.Server.Models.Exceptions
{
    public class GerrymanderringResultsNotFoundException : Exception
    {
        public GerrymanderringResultsNotFoundException() : base("Gerrymanderring results was not found in the database") { }

        public GerrymanderringResultsNotFoundException(string errorMessage) : base(errorMessage) { }

        public GerrymanderringResultsNotFoundException(string errorMessage, Exception innerException)
            : base(errorMessage, innerException) { }
    }
}
