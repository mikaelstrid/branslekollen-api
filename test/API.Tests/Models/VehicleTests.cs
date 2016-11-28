using System;
using API.Models;
using FluentAssertions;
using NUnit.Framework;

namespace API.Tests.Models
{
    // http://www.alteridem.net/2016/06/18/nunit-3-testing-net-core-rc2/
    // https://docs.microsoft.com/en-us/dotnet/articles/core/testing/unit-testing-with-dotnet-test
    [TestFixture]
    public class VehicleTests : BranslekollenAssertionHelper
    {
        private Refueling _refueling1;
        private Refueling _refueling2;
        private Refueling _refueling3;
        private Vehicle _sut;

        [SetUp]
        public void Setup()
        {
            _refueling1 = new Refueling
            {
                Id = "70333769-8F4E-4D4A-8765-27E5D3E19EF8",
                CreationTime = DateTime.Parse("2016-11-10 13:37"),
                MissedRefuelings = false,
                Date = DateTime.Parse("2016-11-10"),
                NumberOfLiters = 45.7,
                PricePerLiter = 13.35,
                OdometerInKm = 45789,
                DistanceTravelledInKm = null,
                FullTank = true
            };
            _refueling2 = new Refueling
            {
                Id = "4DAEC184-BC0D-442F-9AF0-5727785E0E64",
                CreationTime = DateTime.Parse("2016-11-21 08:25"),
                MissedRefuelings = false,
                Date = DateTime.Parse("2016-11-20"),
                NumberOfLiters = 39,
                PricePerLiter = 14.01,
                OdometerInKm = 46507,
                DistanceTravelledInKm = 718,
                FullTank = true
            };
            _refueling3 = new Refueling
            {
                Id = "49ABE3E2-2398-45E8-AAE3-38E4064982A9",
                CreationTime = DateTime.Parse("2016-11-30 21:17"),
                MissedRefuelings = false,
                Date = DateTime.Parse("2016-11-30"),
                NumberOfLiters = 50,
                PricePerLiter = 12.99,
                OdometerInKm = 47490,
                DistanceTravelledInKm = 983,
                FullTank = true
            };
            _sut = new Vehicle
            {
                Id = "C80B05E0-7E05-4C57-8840-F21E5439EB8F",
                Name = "Volvo V90",
                Fuel = Fuel.Diesel,
            };
        }

        [Test]
        public void CalculateFuelConsumption_ShouldReturnUndefinedConsumption_IfNoRefuelings()
        {
            // ARRANGE

            // ACT
            var result = _sut.CalculateFuelConsumption(DateTime.Parse("2016-11-01"), DateTime.Parse("2016-11-30"));

            // ASSERT
            result.Should().NotHaveValue();
        }

        [Test]
        public void CalculateFuelConsumption_ShouldReturnUndefinedConsumption_IfOnlyOneRefueling_AndItIsWithinTimeInterval()
        {
            // ARRANGE
            _sut.Refuelings.Add(_refueling1);

            // ACT
            var result = _sut.CalculateFuelConsumption(DateTime.Parse("2016-11-01"), DateTime.Parse("2016-12-10"));

            // ASSERT
            result.Should().NotHaveValue();
        }

        [Test]
        public void CalculateFuelConsumption_ShouldReturnConsumption_BasedOnLastRefueling_IfTwoRefuelings_BothWithinTimeframe()
        {
            // ARRANGE
            _sut.Refuelings.Add(_refueling1);
            _sut.Refuelings.Add(_refueling2);

            // ACT
            var result = _sut.CalculateFuelConsumption(DateTime.Parse("2016-11-01"), DateTime.Parse("2016-11-30"));

            // ASSERT
            result.Should().BeApproximately(0.0543175487465181, EPSILON);
        }

        [Test]
        public void CalculateFuelConsumption_ShouldReturnConsumption_BasedOnLastTwoRefuelings_IfThreeRefuelings_AllWithinTimeframe()
        {
            // ARRANGE
            _sut.Refuelings.Add(_refueling1);
            _sut.Refuelings.Add(_refueling2);
            _sut.Refuelings.Add(_refueling3);

            // ACT
            var result = _sut.CalculateFuelConsumption(DateTime.Parse("2016-11-01"), DateTime.Parse("2016-12-10"));

            // ASSERT
            result.Should().BeApproximately(0.052591, EPSILON);
        }

        [Test]
        public void CalculateFuelConsumption_ShouldReturnConsumption_BasedOnLastTwoRefuelings_IfThreeRefuelings_AndTheFirstIsBeforeTimeInterval()
        {
            // ARRANGE
            _sut.Refuelings.Add(_refueling1);
            _sut.Refuelings.Add(_refueling2);
            _sut.Refuelings.Add(_refueling3);

            // ACT
            var result = _sut.CalculateFuelConsumption(DateTime.Parse("2016-11-15"), DateTime.Parse("2016-12-10"));

            // ASSERT
            result.Should().BeApproximately(0.052591, EPSILON);
        }

        [Test]
        public void CalculateFuelConsumption_ShouldReturnConsumption_BasedOnLastRefueling_IfThreeRefuelings_AndTheFirstTwoAreBeforeTimeInterval()
        {
            // ARRANGE
            _sut.Refuelings.Add(_refueling1);
            _sut.Refuelings.Add(_refueling2);
            _sut.Refuelings.Add(_refueling3);

            // ACT
            var result = _sut.CalculateFuelConsumption(DateTime.Parse("2016-11-27"), DateTime.Parse("2016-12-10"));

            // ASSERT
            result.Should().BeApproximately(0.0508647, EPSILON);
        }

        [Test]
        public void CalculateFuelConsumption_ShouldReturnUndefinedConsumption_IfThreeRefuelings_AndAllAreBeforeTimeInterval()
        {
            // ARRANGE
            _sut.Refuelings.Add(_refueling1);
            _sut.Refuelings.Add(_refueling2);
            _sut.Refuelings.Add(_refueling3);

            // ACT
            var result = _sut.CalculateFuelConsumption(DateTime.Parse("2016-12-05"), DateTime.Parse("2016-12-10"));

            // ASSERT
            result.Should().NotHaveValue();
        }

        [Test]
        public void CalculateFuelConsumption_ShouldReturnUndefinedConsumption_IfThreeRefuelings_AndAllAreAfterTimeInterval()
        {
            // ARRANGE
            _sut.Refuelings.Add(_refueling1);
            _sut.Refuelings.Add(_refueling2);
            _sut.Refuelings.Add(_refueling3);

            // ACT
            var result = _sut.CalculateFuelConsumption(DateTime.Parse("2016-10-05"), DateTime.Parse("2016-10-31"));

            // ASSERT
            result.Should().NotHaveValue();
        }

        [Test]
        public void CalculateFuelConsumption_ShouldReturnUndefinedConsumption_IfThreeRefuelings_AndOnlyFirstIsWithinTimeInterval()
        {
            // ARRANGE
            _sut.Refuelings.Add(_refueling1);
            _sut.Refuelings.Add(_refueling2);
            _sut.Refuelings.Add(_refueling3);

            // ACT
            var result = _sut.CalculateFuelConsumption(DateTime.Parse("2016-11-01"), DateTime.Parse("2016-11-15"));

            // ASSERT
            result.Should().NotHaveValue();
        }

        [Test]
        public void CalculateFuelConsumption_ShouldReturnConsumption_BasedOnSecondRefueling_IfOnlySecondIsWithinTimeInterval()
        {
            // ARRANGE
            _sut.Refuelings.Add(_refueling1);
            _sut.Refuelings.Add(_refueling2);
            _sut.Refuelings.Add(_refueling3);

            // ACT
            var result = _sut.CalculateFuelConsumption(DateTime.Parse("2016-11-15"), DateTime.Parse("2016-11-25"));

            // ASSERT
            result.Should().BeApproximately(0.054317549, EPSILON);
        }

        [Test]
        public void CalculateFuelConsumption_ShouldReturnConsumption_BasedOnSecondRefueling_IfTimeIntervalIsExactlyTheSameDateAsSecond()
        {
            // ARRANGE
            _sut.Refuelings.Add(_refueling1);
            _sut.Refuelings.Add(_refueling2);
            _sut.Refuelings.Add(_refueling3);

            // ACT
            var result = _sut.CalculateFuelConsumption(DateTime.Parse("2016-11-20"), DateTime.Parse("2016-11-20"));

            // ASSERT
            result.Should().BeApproximately(0.054317549, EPSILON);
        }
    }
}
