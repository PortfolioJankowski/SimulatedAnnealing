using SimulatedAnnealing.Services.Legal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedAnnealing.Models
{
    public class State 
    {
        private readonly ElectoralCodex _electoralCodex = new ElectoralCodex();
        public  Wojewodztwa? ActualConfiguration { get; set; }
        public  Indicator? Indicator { get; set; }

        public double PopulationIndex { get; set; } 

        public int VoivodeshipSeatsAmount { get; set; }

        public int VoivodeshipInhabitants { get; set; }

        // Okreg : <komitet, głosy>
        public  Dictionary<Okregi, Dictionary<string, int>>? DistrictVotingResults { get; set; }

        //Calculate the electoral details based on demographic data from database
        public void CalculateDetails()
        {
            this.CalculateInhabitants();
            this.CalculateVoievodianshipSeatsAmount();
            this.CalculatePopulationIndex();
            this.CalculateDistrictResults();
        }

        private void CalculateDistrictResults()
        {
            this.DistrictVotingResults = _electoralCodex.CalculateResultsForDistricts(this.ActualConfiguration!, this.VoivodeshipSeatsAmount, this.PopulationIndex);
        }

        private void CalculatePopulationIndex()
        {
            this.PopulationIndex = this.VoivodeshipInhabitants / this.VoivodeshipSeatsAmount;
        }

        private void CalculateVoievodianshipSeatsAmount()
        {
            this.VoivodeshipSeatsAmount = _electoralCodex.CalculateSeatsAmountForVoievodianship(this.VoivodeshipInhabitants);
        }

        private void CalculateInhabitants()
        {
            this.VoivodeshipInhabitants = this.ActualConfiguration!.Okregis
                .SelectMany(o => o.Powiaties)
                .Sum(p => p.LiczbaMieszkancow);
        }


    }
}
