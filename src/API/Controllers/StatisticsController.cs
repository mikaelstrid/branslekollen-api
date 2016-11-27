using System;
using System.Threading.Tasks;
using API.ApiModels;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    public class StatisticsController : Controller
    {
        private readonly IVehicleRepository _vehicleRepository;

        public StatisticsController(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        [HttpGet("vehicle/{vehicleId}")]
        public async Task<IActionResult> GetByVehicleId(string vehicleId, DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate) return BadRequest("The start date must occur before the end date.");

            var vehicle = await _vehicleRepository.Find(vehicleId);
            if (vehicle == null)
                return NotFound();

            var result = new FuelConsumptionStatisticsApiModel
            {
                VehicleId = vehicle.Id,
                StartDate = startDate,
                EndDate = endDate,
                FuelConsumptionInLitersPerKm = vehicle.CalculateFuelConsumption(startDate, endDate)
            };

            return new ObjectResult(result);
        }
    }
}
