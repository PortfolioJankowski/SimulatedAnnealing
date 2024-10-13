using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using SimulatedAnnealing.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedAnnealing.Services.Geography
{
    public class Radar
    {
        private SimulatedAnnealingContext _context;

        public Radar(SimulatedAnnealingContext context)
        {
            this._context = context;
        }

        public bool AreCountiesNeighbouring(Powiaty county, int neighborId)
        {
            return county.PowiatySasiadujace.Any(p => p.PowiatId == neighborId);
        }

        public bool IsCountyNeighbouringWithDistrict(Powiaty county, Okregi district)
        {
            foreach (var powiat in district.Powiaties)
            {
                if (AreCountiesNeighbouring(county, powiat.PowiatId))
                {
                    return true;
                }
            }
            return false;
        }


        public bool IsDistrictBoundaryUnbroken(Okregi district)
        {
            if (district == null || district.Powiaties == null || district.Powiaties.Count == 0)
            {
                return false;
            }

            int[] response = new int[district.Powiaties.Count];
            int pointer = 0;
            foreach (var county in district.Powiaties)
            {
                for (int i = 0; i < district.Powiaties.Count; i++)
                {
                    var temp = district.Powiaties.ToArray();
                    if (AreCountiesNeighbouring(county, temp[i].PowiatId))
                    {
                        response[pointer] = 1;
                        break;
                    }
                }
                pointer++;
            }

            for (int j = 0; j < response.Length; j++)
            {
                if (response[j] == 0)
                {
                    return false;
                }
            }

            return true;
        }



    }
}
