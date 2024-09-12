
using Microsoft.EntityFrameworkCore;
using SimulatedAnnealing.Models;
using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

namespace SimulatedAnnealing.Services.Database
{
    public class DbRepository
    {
        private readonly SimulatedAnnealingContext _context;
        public DbRepository(SimulatedAnnealingContext context)
        {
            _context = context;
        }
        public State GetCurrentState()
        {
            // Startpoint => actual electoral situation
            State currentState = new State();
            SimulatedAnnealing.Models.Wojewodztwa? ww =  _context.Wojewodztwas
                .Where(w => w.Nazwa == "małopolskie")
                .Include(w => w.Okregis)
                .ThenInclude(o => o.Powiaties)
                .ThenInclude(p => p.Wynikis
                    .Where(w => w.Rok == 2024))
                 .FirstOrDefault();

            currentState.ActualConfiguration = ww!;
            currentState.ShiftNo = 0;
            currentState.Indicator = new Indicator();
            return currentState;
        }
    }
}
