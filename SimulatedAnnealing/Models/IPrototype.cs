using SimulatedAnnealing.Services.Legal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedAnnealing.Models
{
    public interface IPrototype
    {
         IPrototype Clone();
        
        public Wojewodztwa ActualConfiguration { get; set; }
        public Indicator Indicator { get; set; }

        public double PopulationIndex { get; set; }

        public int VoivodeshipSeatsAmount { get; set; }

        public int VoivodeshipInhabitants { get; set; }

        // Okreg : <komitet, głosy>
        public Dictionary<Okregi, Dictionary<string, int>> DistrictVotingResults { get; set; }

        //Calculate the electoral details based on demographic data from database
        void CalculateDetails();


        void CalculateDistrictResults();


        void CalculatePopulationIndex();


        void CalculateVoievodianshipSeatsAmount();


        void CalculateInhabitants();
        
    }
}
