
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
            try {
                SimulatedAnnealing.Models.Wojewodztwa? ww = _context.Wojewodztwas
                    .Where(w => w.Nazwa == Configuration.ChoosenVoievodenship)
                    .Include(w => w.Okregis)
                        .ThenInclude(o => o.Powiaties)
                            .ThenInclude(p => p.Sasiedzis)
                    .Include(w => w.Okregis)
                        .ThenInclude(p => p.Powiaties)
                            .ThenInclude(p => p.Wynikis
                                .Where(w => w.Rok == 2024 && bestParties.Contains(w.Komitet!)))
                    .FirstOrDefault();

                    if (ww == null){
                        throw new Exception($"No voivodeship founc for {Configuration.ChoosenVoievodenship}");
                    }
                    foreach (var okr in ww.Okregis){
                        foreach( var powiat in okr.Powiaties){
                            var sasiedzi = _context.Sasiedzis
                                .Where(s => s.PowiatId == powiat.PowiatId)
                                .Select(s => s.SasiadId)
                                .ToList();

                            powiat.PowiatySasiadujace = _context.Powiaties
                                .Where(p => sasiedzi.Contains(p.PowiatId))
                                .ToList();
                        }
                    }
                    return ww!;
            } catch(Exception ex) {
                throw new Exception($"An error occuren while fetching the voivodeship data, {ex.Message} ");
            }
        }

        public Wojewodztwa GetVoiewodeshipClone(Dictionary<int, List<int>> neighborConfigurationSettings)
        {
            var bestParties = new List<string>()
            {
                "KOMITET WYBORCZY PRAWO I SPRAWIEDLIWOŚĆ",
                "KOALICYJNY KOMITET WYBORCZY KOALICJA OBYWATELSKA",
                "KOALICYJNY KOMITET WYBORCZY TRZECIA DROGA POLSKA 2050 SZYMONA HOŁOWNI - POLSKIE STRONNICTWO LUDOWE",
                "KOMITET WYBORCZY WYBORCÓW KONFEDERACJA I BEZPARTYJNI SAMORZĄDOWCY"
            };

            var ww = _context.Wojewodztwas
                .FirstOrDefault(w => w.Nazwa == Configuration.ChoosenVoievodenship);

            if (ww == null)
            {
                throw new Exception($"No voivodeship found with the name {Configuration.ChoosenVoievodenship}");
            }
            ww.Okregis = new List<Okregi>();

            var powiaty = _context.Powiaties
                .Include(p => p.Wynikis
                    .Where(w => w.Rok == 2024 && bestParties.Contains(w.Komitet!)))
                .ToList(); 

            var sasiedziDict = _context.Sasiedzis
                .GroupBy(s => s.PowiatId ?? 0)
                .ToDictionary(g => g.Key, g => g.Select(s=> s.SasiadId).ToList());

            foreach (var kvp in neighborConfigurationSettings)
            {
                int districtId = kvp.Key;
                var powiatIdsInDistrict = kvp.Value;

                // Get all powiats that belong to the current district
                var powiatsInDistrict = powiaty.Where(p => powiatIdsInDistrict.Contains(p.PowiatId)).ToList();

                if (powiatsInDistrict.Count == 0)
                {
                    throw new Exception($"Found district {districtId} without any counties!");
                }

                // Assign neighbors (PowiatySasiadujace) for each powiat in this district
                foreach (var powiat in powiatsInDistrict)
                {
                    if (sasiedziDict.TryGetValue(powiat.PowiatId, out var neighborIds))
                    {
                        // Only assign neighbors that are part of the fetched powiaty list
                        powiat.PowiatySasiadujace = powiaty
                            .Where(p => neighborIds.Contains(p.PowiatId))
                            .ToList(); // No need for casting
                    }
                    else
                    {
                        powiat.PowiatySasiadujace = new List<Powiaty>(); // Assign empty list if no neighbors found
                    }
                }

                // Create the new district (Okregi) and assign its powiats
                var okr = new Okregi
                {
                    OkregId = districtId,
                    Wojewodztwo = ww,
                    WojewodztwoId = ww.WojewodztwoId,
                    Powiaties = powiatsInDistrict
                };

                ww.Okregis.Add(okr); // Add the district to the voivodeship
            }

            // Return the modified voivodeship with the newly created districts
            return ww;
        }


        internal Wojewodztwa GetVoievodenship()
        {
            throw new NotImplementedException();
        }
    }
}
