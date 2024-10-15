using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SimulatedAnnealing.Models;
using SimulatedAnnealing.Services.Geography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedAnnealing.Tests.Integration
{
    public class RadarTest
    {

        [Theory]
        [InlineData(new string[] { "Olkuski", "Miechowski", "Proszowicki", "Myślenicki", "Wadowicki" }, false)] // Sad path: Inappropriate district
        [InlineData(new string[] { "Miechowski", "Proszowicki", "Brzeski", "Nowy Sącz", "Nowosądecki" }, true)] // Happy path: Appropriate district

        public void IsDistrictBoundaryUnbroken_ShouldReturnExpectedResult_WhenGivenDistrict(string[] chosenCounties, bool expectedResult)
        {
            // Arrange
            using var context = new SimulatedAnnealingContext();
            var counties = context.Powiaties.Where(c => chosenCounties.Contains(c.Nazwa)).ToList();
            var allCounties = context.Powiaties.ToList();


            var district = new Okregi { Powiaties = counties };
            var neighborsDict = context.Sasiedzis
                .GroupBy(s => s.PowiatId ?? 0)
                .ToDictionary(g => g.Key, g => g.Select(s => s.SasiadId).ToList());

            foreach (var powiat in district.Powiaties)
            {
                powiat.PowiatySasiadujace = neighborsDict.TryGetValue(powiat.PowiatId, out var neighborIds)
                    ? allCounties.Where(p => neighborIds.Contains(p.PowiatId)).ToList()
                    : new List<Powiaty>();
            }

            // Act
            var radar = new Radar(context);
            bool result = radar.IsDistrictBoundaryUnbroken(district);

            // Assert the expected result
            result.Should().Be(expectedResult);
        }



    }
}


