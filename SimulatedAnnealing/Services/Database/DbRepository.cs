
using Microsoft.EntityFrameworkCore;
using SimulatedAnnealing.Models;
using SimulatedAnnealing.Services.Config;
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
        public Wojewodztwa GetInitialVoievodeship()
        {
            var bestParties = new List<string>()
            {
                "KOMITET WYBORCZY PRAWO I SPRAWIEDLIWOŚĆ","KOALICYJNY KOMITET WYBORCZY KOALICJA OBYWATELSKA" ,"KOALICYJNY KOMITET WYBORCZY TRZECIA DROGA POLSKA 2050 SZYMONA HOŁOWNI - POLSKIE STRONNICTWO LUDOWE","KOMITET WYBORCZY WYBORCÓW KONFEDERACJA I BEZPARTYJNI SAMORZĄDOWCY"
            };

            // Startpoint => actual electoral situation
            SimulatedAnnealing.Models.Wojewodztwa? ww = _context.Wojewodztwas
                .Where(w => w.Nazwa == Configuration.ChoosenVoievodenship)
                .Include(w => w.Okregis)
                .ThenInclude(o => o.Powiaties)
                .ThenInclude(p => p.Wynikis
                    .Where(w => w.Rok == 2024 && bestParties.Contains(w.Komitet)))
                 .FirstOrDefault();
            return ww!;
        }

        public Wojewodztwa GetVoiewodeshipClone(Dictionary<int, List<int>> neighborConfigurationSettings)
        {
            var bestParties = new List<string>()
            {
                "KOMITET WYBORCZY PRAWO I SPRAWIEDLIWOŚĆ","KOALICYJNY KOMITET WYBORCZY KOALICJA OBYWATELSKA" ,"KOALICYJNY KOMITET WYBORCZY TRZECIA DROGA POLSKA 2050 SZYMONA HOŁOWNI - POLSKIE STRONNICTWO LUDOWE","KOMITET WYBORCZY WYBORCÓW KONFEDERACJA I BEZPARTYJNI SAMORZĄDOWCY"
            };

            var ww = _context.Wojewodztwas.First(w => w.Nazwa == Configuration.ChoosenVoievodenship);
            var powiaty = _context.Powiaties
                .Include(p => p.Wynikis
                    .Where(w => w.Rok == 2024 && bestParties.Contains(w.Komitet)))
                .AsQueryable();
            
            foreach (int nr in neighborConfigurationSettings.Keys){
                var okr = new Okregi
                {
                    OkregId = nr,
                    Wojewodztwo = ww,
                    WojewodztwoId = ww.WojewodztwoId,
                    Powiaties = powiaty.Where(p => neighborConfigurationSettings[nr].Contains(p.PowiatId)).ToList()
                };
                ww.Okregis.Add(okr);
            }
            return ww;
        }

        internal Wojewodztwa GetVoievodenship()
        {
            throw new NotImplementedException();
        }
    }
}
