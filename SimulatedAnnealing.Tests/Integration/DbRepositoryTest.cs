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
        [Fact]
        public void GetVoiewodeshipClone_ShouldReturnVoivodeship_WhenDataIsValid()
        {
            // Arrange
            var wojewodztwa = new Wojewodztwa { Nazwa = "małopolskie", WojewodztwoId = 1 };
            var data = new List<Wojewodztwa> { wojewodztwa }.AsQueryable();

            var mockWojewodztwaSet = new Mock<DbSet<Wojewodztwa>>();
            mockWojewodztwaSet.As<IQueryable<Wojewodztwa>>().Setup(m => m.Provider).Returns(data.Provider);
            mockWojewodztwaSet.As<IQueryable<Wojewodztwa>>().Setup(m => m.Expression).Returns(data.Expression);
            mockWojewodztwaSet.As<IQueryable<Wojewodztwa>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockWojewodztwaSet.As<IQueryable<Wojewodztwa>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            _mockContext.Setup(c => c.Wojewodztwas).Returns(mockWojewodztwaSet.Object);

            var powiaty = new List<Powiaty> { new Powiaty { PowiatId = 1, Nazwa = "Test Powiat" } }.AsQueryable();
            var mockPowiatySet = new Mock<DbSet<Powiaty>>();
            mockPowiatySet.As<IQueryable<Powiaty>>().Setup(m => m.Provider).Returns(powiaty.Provider);
            mockPowiatySet.As<IQueryable<Powiaty>>().Setup(m => m.Expression).Returns(powiaty.Expression);
            mockPowiatySet.As<IQueryable<Powiaty>>().Setup(m => m.ElementType).Returns(powiaty.ElementType);
            mockPowiatySet.As<IQueryable<Powiaty>>().Setup(m => m.GetEnumerator()).Returns(powiaty.GetEnumerator());
            _mockContext.Setup(c => c.Powiaties).Returns(mockPowiatySet.Object);

            var sasiedzi = new List<Sasiedzi>().AsQueryable();
            var mockSasiedziSet = new Mock<DbSet<Sasiedzi>>();
            mockSasiedziSet.As<IQueryable<Sasiedzi>>().Setup(m => m.Provider).Returns(sasiedzi.Provider);
            mockSasiedziSet.As<IQueryable<Sasiedzi>>().Setup(m => m.Expression).Returns(sasiedzi.Expression);
            mockSasiedziSet.As<IQueryable<Sasiedzi>>().Setup(m => m.ElementType).Returns(sasiedzi.ElementType);
            mockSasiedziSet.As<IQueryable<Sasiedzi>>().Setup(m => m.GetEnumerator()).Returns(sasiedzi.GetEnumerator());
            _mockContext.Setup(c => c.Sasiedzis).Returns(mockSasiedziSet.Object);

            var neighborConfig = new Dictionary<int, List<int>> { { 1, new List<int> { 1 } } };

            // Act
            var result = _repository.GetVoiewodeshipClone(neighborConfig);

            // Assert
            result.Should().NotBeNull();
            result.Nazwa.Should().Be("małopolskie");
            result.Okregis.Should().HaveCount(1);
        }
    }
}
