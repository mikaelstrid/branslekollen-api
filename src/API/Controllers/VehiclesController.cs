using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.ApiModels;
using API.DAL;
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

            Vehicle domainVehicle;
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


        [HttpPost("add-refueling/{id}")]
        public async Task<IActionResult> Create(string id, [FromBody] RefuelingApiModel refueling)
        {
            var domainVehicle = await _vehicleRepository.Find(id);
            if (domainVehicle == null)
                return BadRequest();
            refueling.Id = Guid.NewGuid().ToString();

            var domainRefueling = refueling.ToDomainModel();
            domainVehicle.Refuelings.Add(domainRefueling);

            _vehicleRepository.Update(domainVehicle);
            return new NoContentResult();
        }
    }
}
