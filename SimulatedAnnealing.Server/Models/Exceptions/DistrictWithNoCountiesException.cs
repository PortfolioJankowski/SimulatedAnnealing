namespace SimulatedAnnealing.Server.Models.Exceptions
{
    public class DistrictWithNoCountiesException : Exception
    {
        public DistrictWithNoCountiesException() : base() { }
        public DistrictWithNoCountiesException(string message) : base(message) { }
        public DistrictWithNoCountiesException(string message,  Exception innerException) : base(message, innerException) { }
    }
}
