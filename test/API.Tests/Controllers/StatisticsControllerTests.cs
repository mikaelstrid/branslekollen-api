using System;
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
    public class StatisticsControllerTests : BranslekollenAssertionHelper
    {
        private Mock<IVehicleRepository> _vehicleRepository;
        private StatisticsController _sut;

        private Mock<IVehicle> _vehicle1;

        [SetUp]
        public void Setup()
        {
            _vehicle1 = new Mock<IVehicle>();
            _vehicle1.SetupGet(m => m.Id).Returns("115C6883-814F-4485-8AFF-51D44321E3D0");
            _vehicleRepository = new Mock<IVehicleRepository>();
            _vehicleRepository.Setup(r => r.Find(It.IsAny<string>())).Returns(Task.FromResult(_vehicle1.Object));
            _sut = new StatisticsController(_vehicleRepository.Object);
        }

        [Test]
        public async Task GetByVehicleId_ShouldReturnBadRequest_IfStartDate_IsAfterEndDate()
        {
            // ARRANGE

            // ACT
            var result = await _sut.GetByVehicleId(_vehicle1.Object.Id, DateTime.Parse("2016-11-26"), DateTime.Parse("2016-11-20"));

            // ASSERT
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetByVehicleId_ShouldReturnNotFound_IfVehicleWithMatchingId_DoesNotExist()
        {
            // ARRANGE
            _vehicleRepository.Setup(r => r.Find(It.IsAny<string>())).Returns(Task.FromResult<IVehicle>(null));

            // ACT
            var result = await _sut.GetByVehicleId(_vehicle1.Object.Id, DateTime.Parse("2016-11-01"), DateTime.Parse("2016-11-30"));

            // ASSERT
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task GetByVehicleId_ShouldReturnConsumptionFromVehicle_IfVehicleExist()
        {
            // ARRANGE
            const double consumption = 0.5345;
            var startDate = DateTime.Parse("2016-11-01");
            var endDate = DateTime.Parse("2016-11-30");
            _vehicle1.Setup(m => m.CalculateFuelConsumption(startDate, endDate)).Returns(consumption);

            // ACT
            var result = await _sut.GetByVehicleId(_vehicle1.Object.Id, startDate, endDate);

            // ASSERT
            Expect((ObjectResult) result, Is.EqualTo(new FuelConsumptionStatisticsApiModel(_vehicle1.Object.Id, startDate, endDate, consumption)));
            _vehicle1.Verify(m => m.CalculateFuelConsumption(startDate, endDate), Times.Once);
        }
    }
}
