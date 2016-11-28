using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.ApiModels;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    public class VehiclesController : Controller
    {
        private readonly IVehicleRepository _vehicleRepository;

        public VehiclesController(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<VehicleApiModel>> GetAll()
        {
            return (await _vehicleRepository.GetAll()).Select(VehicleApiModel.FromDomainModel);
        }

        [HttpGet("{id}", Name = "GetVehicle")]
        public async Task<IActionResult> GetById(string id)
        {
            var vehicle = await _vehicleRepository.Find(id);
            if (vehicle == null)
                return NotFound();
            return new ObjectResult(VehicleApiModel.FromDomainModel(vehicle));
        }

        [HttpGet("ids")]
        public async Task<IActionResult> GetByIds([FromQuery] string[] ids)
        {
            if (ids == null) return BadRequest("The ids parameter must not be null");

            var result = new List<VehicleApiModel>();
            foreach (var id in ids)
            {
                var vehicle = await _vehicleRepository.Find(id);
                if (vehicle != null) result.Add(VehicleApiModel.FromDomainModel(vehicle));
            }

            return Ok(result);
        }

        [HttpPost]
        public IActionResult Create([FromBody] VehicleApiModel vehicle)
        {
            if (vehicle == null)
                return BadRequest();
            vehicle.Id = Guid.NewGuid().ToString();

            IVehicle domainVehicle;
            try { domainVehicle = vehicle.ToDomainModel(); } catch (Exception e) { return BadRequest(e.Message); }
            _vehicleRepository.Add(domainVehicle);
            return CreatedAtRoute("GetVehicle", new { id = domainVehicle.Id }, VehicleApiModel.FromDomainModel(domainVehicle));
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] VehicleApiModel vehicle)
        {
            if (vehicle == null || vehicle.Id != id)
                return BadRequest();

            var existingVehicle = _vehicleRepository.Find(id);
            if (existingVehicle == null)
                return NotFound();

            _vehicleRepository.Update(vehicle.ToDomainModel());
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var existingVehicle = _vehicleRepository.Find(id);
            if (existingVehicle == null)
                return NotFound();

            _vehicleRepository.Remove(id);
            return new NoContentResult();
        }


        [HttpPost("refueling/{vehicleId}")]
        public async Task<IActionResult> AddRefueling(string vehicleId, [FromBody] RefuelingApiModel refueling)
        {
            var domainVehicle = await _vehicleRepository.Find(vehicleId);
            if (domainVehicle == null)
                return BadRequest();

            refueling.Id = Guid.NewGuid().ToString();
            refueling.CreationTime = DateTime.UtcNow;

            var domainRefueling = refueling.ToDomainModel();
            domainVehicle.Refuelings.Add(domainRefueling);
            domainVehicle.Refuelings.Sort((r1, r2) => r1.Date.CompareTo(r2.Date));

            UpdatedTravelledDistances(domainVehicle.Refuelings);

            _vehicleRepository.Update(domainVehicle);
            return new NoContentResult();
        }

        [HttpPut("refueling/{vehicleId}")]
        public async Task<IActionResult> UpdateRefueling(string vehicleId, [FromBody] RefuelingApiModel refueling)
        {
            var domainVehicle = await _vehicleRepository.Find(vehicleId);
            if (domainVehicle == null)
                return BadRequest();

            if (domainVehicle.Refuelings.All(r => r.Id != refueling.Id))
                return NotFound();

            domainVehicle.Refuelings.RemoveAll(r => r.Id == refueling.Id);

            var domainRefueling = refueling.ToDomainModel();
            domainVehicle.Refuelings.Add(domainRefueling);
            domainVehicle.Refuelings.Sort((r1, r2) => r1.Date.CompareTo(r2.Date));

            UpdatedTravelledDistances(domainVehicle.Refuelings);

            _vehicleRepository.Update(domainVehicle);
            return new NoContentResult();
        }

        private static void UpdatedTravelledDistances(IEnumerable<Refueling> refuelings)
        {
            Refueling previousRefueling = null;
            foreach (var refueling in refuelings)
            {
                if (previousRefueling == null)
                    refueling.DistanceTravelledInKm = null;
                else
                    refueling.DistanceTravelledInKm = refueling.OdometerInKm - previousRefueling.OdometerInKm;

                previousRefueling = refueling;
            }
        }
    }
}
