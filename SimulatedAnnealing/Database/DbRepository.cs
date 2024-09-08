
using Microsoft.EntityFrameworkCore;
using SimulatedAnnealing.Models;
using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

namespace SimulatedAnnealing.Database
{
    public class DbRepository
    {
        private SimulatedAnnealingContext _context = new();
        public DbRepository(SimulatedAnnealingContext context)
        {
            _context = context;
        }
        public async Task<State> GetCurrentStateAsync()
        {
            // Startpoint
            State currentState = new State();
            var wojewodztwo = await _context.Wojewodztwas
                .Where(w => w.Nazwa == "Małopolskie")
                .Include(w => w.Okregis)
                    .ThenInclude(o => o.Powiaties)
                        .ThenInclude(p => p.Wynikis
                        .Where(wynik => wynik.Rok == 2024)
                    ).FirstOrDefaultAsync();

            currentState.ActualConfiguration = wojewodztwo;
            currentState.ShiftNo = 0;
            currentState.Indicator = new Indicator();

            return currentState;
        }
    }
}
