﻿using SimulatedAnnealing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedAnnealing.Calculator
{
    public class Calculator
    {
        public Indicator setNewIndicator()
        {
            return new Indicator();
        }
        public bool isNewIndicatorBetter()
        {
            return true;
        }

        public void chooseTheBestDistrict()
        {

        }

        public void chooseTheWorstDistrict()
        {

        }
    }
}