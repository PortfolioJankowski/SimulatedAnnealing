namespace SimulatedAnnealing.Server.Models.Exceptions
{
    public class VoivodeshipNotFoundException : Exception
    {
        public VoivodeshipNotFoundException() : base("The specified voivodeship was not found.")

        { }
        public VoivodeshipNotFoundException(string message)
            : base(message)
        {
        }

        public VoivodeshipNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
