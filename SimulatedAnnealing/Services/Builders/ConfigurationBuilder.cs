using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedAnnealing.Services.Builders
{
    public class ConfigurationBuilder
    {
        public Configuration Build()
        {
            return new Configuration()
            {
                InitialSolution = 10.0,
                InitialTemperature = 100.0,
                CoolingRate = 0.99,
                StepSize = 1.0,
                MaxIterations = 1000
            };
        }
    }

    public class Configuration
    {
        public double InitialSolution { get; set; }
        public double InitialTemperature { get; set; }
        public double CoolingRate { get; set; }
        public double StepSize { get; set; }
        public int MaxIterations { get; set; }

    }
}
