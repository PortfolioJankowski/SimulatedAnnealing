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
        private readonly DbRepository _dbRepository;
        private readonly Predictor _predictor;  

        public StateBuilder(DbRepository dbRepository, Predictor calculator) 
        {
            _dbRepository = dbRepository;
            _predictor = calculator;   
        }
        public void GetVoievodenship(bool isInitialState)
        {
            _state.ActualConfiguration = isInitialState ?  _dbRepository.GetInitialVoievodeship() : _dbRepository.GetVoievodenship();
        }
        private void SetVotingResult()
        {
            _state.CalculateDetails();
        }

        private void SetIndicator()
        {
            _state.Indicator = _predictor.setNewIndicator(_state.ActualConfiguration);
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
