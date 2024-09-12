using SimulatedAnnealing.Models;
using SimulatedAnnealing.Services.Database;
using SimulatedAnnealing.Services.Legal;
using SimulatedAnnealing.Services.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedAnnealing.Services.Builders
{
    public class StateBuilder
    {
        private State _state = new State();
        private readonly DHondt _dhondt;
        private readonly DbRepository _dbRepository;
        private readonly Calculator _calculator;  

        public StateBuilder(DHondt dHondt, DbRepository dbRepository, Calculator calculator) 
        {
            _dhondt = dHondt;
            _dbRepository = dbRepository;
            _calculator = calculator;   
        }
        public void GetVoievodenship(bool isInitialState)
        {
            _state.ActualConfiguration = isInitialState ?  _dbRepository.GetInitialVoievodeship() : _dbRepository.GetVoievodenship();
        }
        private void SetVotingResult()
        {
            _state.VotingResults = _dhondt.CalculateVotingResults(_state);
        }

        private void SetIndicator()
        {
            _state.Indicator = _calculator.setNewIndicator(_state.VotingResults);
        }

        public State Build(bool isInitialState)
        {
            this.GetVoievodenship(isInitialState);
            this.SetVotingResult();
            this.SetIndicator();

            return _state;
        }

       



    }
}
