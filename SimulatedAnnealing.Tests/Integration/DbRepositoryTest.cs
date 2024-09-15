using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SimulatedAnnealing.Models;
using SimulatedAnnealing.Services.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedAnnealing.Tests.Integration
{
    public class DbRepositoryTest
    {
        private readonly DbRepository _repository;
        private readonly Mock<SimulatedAnnealingContext> _mockContext;

        public DbRepositoryTest()
        {
            _mockContext = new Mock<SimulatedAnnealingContext>();
            _repository = new DbRepository(_mockContext.Object);
        }
        [Fact]
        public void GetInitialVoievodeship_ShouldReturnVoievodeship_WhenDataIsValid()
        {
            // Arrange
            var okreg = new Okregi { WojewodztwoId = 1 , OkregId=1};
            var okregi = new List<Okregi>
            {
                okreg
            };
            var wojewodztwa = new Wojewodztwa { Nazwa = "małopolskie", WojewodztwoId = 1, Okregis = (ICollection<Okregi>)okregi };
            //creating queryable List
            var data = new List<Wojewodztwa> { wojewodztwa }.AsQueryable();
            
            //mocking list with setup
            var mockWojewodztwaSet = new Mock<DbSet<Wojewodztwa>>();
            mockWojewodztwaSet.As<IQueryable<Wojewodztwa>>().Setup(m => m.Provider).Returns(data.Provider); //data goes here
            mockWojewodztwaSet.As<IQueryable<Wojewodztwa>>().Setup(m => m.Expression).Returns(data.Expression);
            mockWojewodztwaSet.As<IQueryable<Wojewodztwa>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockWojewodztwaSet.As<IQueryable<Wojewodztwa>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            // dbContext should return mocked DBSet
            _mockContext.Setup(c => c.Wojewodztwas).Returns(mockWojewodztwaSet.Object);

            // Act
            var result = _repository.GetInitialVoievodeship();

            // Assert
            result.Should().NotBeNull();
            result.Nazwa.Should().Be("małopolskie");
            result.Okregis.Should().NotBeEmpty();
        }


        [Fact]
        public void GetInitialVoievodeship_ShouldReturnNull_WhenNoMatchingData()
        {
            // Arrange
            var data = new List<Wojewodztwa>().AsQueryable();

            var mockSet = new Mock<DbSet<Wojewodztwa>>();
            mockSet.As<IQueryable<Wojewodztwa>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Wojewodztwa>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Wojewodztwa>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Wojewodztwa>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            _mockContext.Setup(c => c.Wojewodztwas).Returns(mockSet.Object);

            // Act
            var result = _repository.GetInitialVoievodeship();

            // Assert
            Assert.Null(result);
        }
    }
}
