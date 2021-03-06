using System;
using System.Linq;
using System.Threading.Tasks;
using API.ApiModels;
using API.Controllers;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace API.Tests.Controllers
{
    // http://www.alteridem.net/2016/06/18/nunit-3-testing-net-core-rc2/
    // https://docs.microsoft.com/en-us/dotnet/articles/core/testing/unit-testing-with-dotnet-test
    [TestFixture]
    public class VehiclesControllerTests : BranslekollenAssertionHelper
    {
        private Mock<IVehicleRepository> _vehicleRepository;
        private VehiclesController _sut;

        private RefuelingApiModel _refuelingApiModel1;
        private RefuelingApiModel _refuelingApiModel2;
        private RefuelingApiModel _refuelingApiModel3;
        private IVehicle _vehicle1;

        [SetUp]
        public void Setup()
        {
            _refuelingApiModel1 = new RefuelingApiModel
            {
                Id = "70333769-8F4E-4D4A-8765-27E5D3E19EF8",
                MissedRefuelings = false,
                Date = DateTime.Parse("2016-11-23"),
                NumberOfLiters = 45.7,
                PricePerLiter = 13.35,
                OdometerInKm = 45789,
                FullTank = true
            };
            _refuelingApiModel2 = new RefuelingApiModel
            {
                Id = "4DAEC184-BC0D-442F-9AF0-5727785E0E64",
                MissedRefuelings = false,
                Date = DateTime.Parse("2016-12-17"),
                NumberOfLiters = 39,
                PricePerLiter = 14.01,
                OdometerInKm = 46507,
                FullTank = true
            };
            _refuelingApiModel3 = new RefuelingApiModel
            {
                Id = "49ABE3E2-2398-45E8-AAE3-38E4064982A9",
                MissedRefuelings = false,
                Date = DateTime.Parse("2016-12-30"),
                NumberOfLiters = 50,
                PricePerLiter = 12.99,
                OdometerInKm = 47490,
                FullTank = true
            };
            _vehicle1 = new Vehicle
            {
                Id = "C80B05E0-7E05-4C57-8840-F21E5439EB8F",
                Name = "Volvo V90",
                Fuel = Fuel.Diesel,
            };
            _vehicleRepository = new Mock<IVehicleRepository>();
            _vehicleRepository.Setup(r => r.Find(_vehicle1.Id)).Returns(Task.FromResult(_vehicle1));

            _sut = new VehiclesController(_vehicleRepository.Object);
        }

        [Test]
        public async Task AddRefueling_ShouldReturnBadRequest_IfVehicleWithMatchingId_DoesNotExist()
        {
            // ARRANGE
            _vehicleRepository.Setup(r => r.Find(_vehicle1.Id)).Returns(Task.FromResult<IVehicle>(null));

            // ACT
            var result = await _sut.AddRefueling(_vehicle1.Id, _refuelingApiModel1);

            // ASSERT
            Assert.That(result, Is.TypeOf<BadRequestResult>());
        }

        [Test]
        public async Task AddRefueling_ShouldAddNewRefueling_WithUndefinedDistance_IfFirstRefueling()
        {
            // ARRANGE

            // ACT
            await _sut.AddRefueling(_vehicle1.Id, _refuelingApiModel1);

            // ASSERT
            _vehicleRepository.Verify(r => r.Update(
                It.Is<Vehicle>(v => 
                    v.Id == _vehicle1.Id
                    && v.Refuelings.First().Date == _refuelingApiModel1.Date
                    && !v.Refuelings.First().DistanceTravelledInKm.HasValue)));
        }
        
        [Test]
        public async Task AddRefueling_ShouldAddNewRefueling_WithDefinedDistance_ThatIsAddedLast_IfSecondRefuelingWithLaterDate()
        {
            // ARRANGE
            _vehicle1.Refuelings.Add(_refuelingApiModel1.ToDomainModel());

            // ACT
            await _sut.AddRefueling(_vehicle1.Id, _refuelingApiModel2);

            // ASSERT
            _vehicleRepository.Verify(r => r.Update(
                It.Is<Vehicle>(v =>
                    v.Id == _vehicle1.Id
                    && v.Refuelings.First().Date == _refuelingApiModel1.Date
                    && !v.Refuelings.First().DistanceTravelledInKm.HasValue
                    && v.Refuelings.Skip(1).First().Date == _refuelingApiModel2.Date
                    && v.Refuelings.Skip(1).First().DistanceTravelledInKm.Value == 718)));
        }

        [Test]
        public async Task AddRefueling_ShouldAddNewRefueling_WithUndefinedDistance_ThatIsAddedFirst_IfSecondRefuelingWithPriorDate()
        {
            // ARRANGE
            _vehicle1.Refuelings.Add(_refuelingApiModel2.ToDomainModel());

            // ACT
            await _sut.AddRefueling(_vehicle1.Id, _refuelingApiModel1);

            // ASSERT
            _vehicleRepository.Verify(r => r.Update(
                It.Is<Vehicle>(v =>
                    v.Id == _vehicle1.Id
                    && v.Refuelings.First().Date == _refuelingApiModel1.Date
                    && !v.Refuelings.First().DistanceTravelledInKm.HasValue
                    && v.Refuelings.Skip(1).First().Date == _refuelingApiModel2.Date
                    && v.Refuelings.Skip(1).First().DistanceTravelledInKm.Value == 718)));
        }

        [Test]
        public async Task AddRefueling_ShouldAddNewRefueling_WithDefinedDistance_ThatIsAddedInTheMiddle_IfThirdRefuelingBetweenFirstAndSecond()
        {
            // ARRANGE
            _vehicle1.Refuelings.Add(_refuelingApiModel1.ToDomainModel());
            _vehicle1.Refuelings.Add(_refuelingApiModel3.ToDomainModel());
            _vehicle1.Refuelings[1].DistanceTravelledInKm = _refuelingApiModel3.OdometerInKm - _refuelingApiModel1.OdometerInKm;

            // ACT
            await _sut.AddRefueling(_vehicle1.Id, _refuelingApiModel2);

            // ASSERT
            _vehicleRepository.Verify(r => r.Update(
                It.Is<Vehicle>(v =>
                    v.Id == _vehicle1.Id
                    && v.Refuelings.First().Date == _refuelingApiModel1.Date
                    && !v.Refuelings.First().DistanceTravelledInKm.HasValue
                    && v.Refuelings.Skip(1).First().Date == _refuelingApiModel2.Date
                    && v.Refuelings.Skip(1).First().DistanceTravelledInKm.Value == 718
                    && v.Refuelings.Skip(2).First().Date == _refuelingApiModel3.Date
                    && v.Refuelings.Skip(2).First().DistanceTravelledInKm.Value == 983)));
        }


        // NOT HAPPY CASE, vi tar detta senare tillsammans med support för icke-full tankning
        //[Test]
        //public async Task AddRefueling_ShouldAddNewRefueling_WithUndefinedDistance_IfNotFullTank()
        //{
        //    // ARRANGE
        //    _vehicle1.Refuelings.Add(_refuelingApiModel3.ToDomainModel());

        //    // ACT
        //    await _sut.AddRefueling(_vehicle1.Id, _refuelingApiModel4NotFullTank);

        //    // ASSERT
        //    _vehicleRepository.Verify(r => r.Update(
        //        It.Is<Vehicle>(v =>
        //            v.Id == _vehicle1.Id
        //            && v.Refuelings.First().Date == _refuelingApiModel3.Date
        //            && !v.Refuelings.First().DistanceTravelledInKm.HasValue
        //            && v.Refuelings.Skip(1).First().Date == _refuelingApiModel4NotFullTank.Date
        //            && !v.Refuelings.Skip(1).First().DistanceTravelledInKm.HasValue)));
        //}


        [Test]
        public async Task UpdateRefueling_ShouldReturnBadRequest_IfVehicleWithMatchingId_DoesNotExist()
        {
            // ARRANGE
            _vehicleRepository.Setup(r => r.Find(_vehicle1.Id)).Returns(Task.FromResult<IVehicle>(null));

            // ACT
            var result = await _sut.UpdateRefueling(_vehicle1.Id, _refuelingApiModel1);

            // ASSERT
            Assert.That(result, Is.TypeOf<BadRequestResult>());
        }

        [Test]
        public async Task UpdateRefueling_ShouldReturnNotFound_IfVehicleHasNoRefuelings()
        {
            // ARRANGE

            // ACT
            var result = await _sut.UpdateRefueling(_vehicle1.Id, _refuelingApiModel1);

            // ASSERT
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task UpdateRefueling_ShouldReturnNotFound_IfRefuelingWithMatchingId_DoesNotExist()
        {
            // ARRANGE
            _refuelingApiModel1.Id = "64DA6746-B301-4438-9053-1B0F978DD8C8";
            _refuelingApiModel2.Id = "11F96DB5-177C-42B0-947D-03525B862C98";
            _vehicle1.Refuelings.Add(_refuelingApiModel1.ToDomainModel());

            // ACT
            var result = await _sut.UpdateRefueling(_vehicle1.Id, _refuelingApiModel2);

            // ASSERT
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task UpdateRefueling_ShouldUpdateRefueling_IfRefuelingWithMatchingId_Exists()
        {
            // ARRANGE
            _refuelingApiModel2.Id = "11F96DB5-177C-42B0-947D-03525B862C98";
            var refuelingApiModel2Updated = _refuelingApiModel2.Clone();
            refuelingApiModel2Updated.NumberOfLiters = 45;
            _vehicle1.Refuelings.Add(_refuelingApiModel2.ToDomainModel());

            // ACT
            await _sut.UpdateRefueling(_vehicle1.Id, refuelingApiModel2Updated);

            // ASSERT
            _vehicleRepository.Verify(r => r.Update(
                It.Is<Vehicle>(v =>
                    v.Id == _vehicle1.Id
                    && v.Refuelings.First().Id == _refuelingApiModel2.Id
                    && v.Refuelings.First().OdometerInKm == _refuelingApiModel2.OdometerInKm
                    && Math.Abs(v.Refuelings.First().NumberOfLiters - refuelingApiModel2Updated.NumberOfLiters) < EPSILON)));
        }

        [Test]
        public async Task UpdateRefueling_ShouldChangeRefuelingOrder_IfRefuelingDateChanges()
        {
            // ARRANGE
            _refuelingApiModel1.Id = "2F4B74E4-BB18-447C-BDC5-2E13835EBAB9";
            _refuelingApiModel2.Id = "11F96DB5-177C-42B0-947D-03525B862C98";

            _vehicle1.Refuelings.Add(_refuelingApiModel1.ToDomainModel());
            _vehicle1.Refuelings.Add(_refuelingApiModel2.ToDomainModel());

            var refuelingApiModel2Updated = _refuelingApiModel2.Clone();
            refuelingApiModel2Updated.Date = _refuelingApiModel1.Date.AddDays(-1);

            // ACT
            await _sut.UpdateRefueling(_vehicle1.Id, refuelingApiModel2Updated);

            // ASSERT
            _vehicleRepository.Verify(r => r.Update(
                It.Is<Vehicle>(v =>
                    v.Id == _vehicle1.Id
                    && v.Refuelings.First().Id == _refuelingApiModel2.Id
                    && v.Refuelings.First().Date == refuelingApiModel2Updated.Date
                    && !v.Refuelings.First().DistanceTravelledInKm.HasValue
                    && v.Refuelings.Skip(1).First().Id == _refuelingApiModel1.Id
                    && v.Refuelings.Skip(1).First().Date == _refuelingApiModel1.Date
                    && v.Refuelings.Skip(1).First().DistanceTravelledInKm.HasValue)));
        }

    }
}
