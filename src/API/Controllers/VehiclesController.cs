using System.Collections.Generic;
using System.Linq;
using API.ApiModels;
using API.DAL;
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
        public IEnumerable<VehicleApiModel> GetAll()
        {
            return _vehicleRepository.GetAll().Select(VehicleApiModel.FromDomainModel);
        }

        [HttpGet("{id}", Name = "GetVehicle")]
        public IActionResult GetById(string id)
        {
            var vehicle = _vehicleRepository.Find(id);
            if (vehicle == null)
                return NotFound();
            return new ObjectResult(VehicleApiModel.FromDomainModel(vehicle));
        }

        [HttpPost]
        public IActionResult Create([FromBody] VehicleApiModel vehicle)
        {
            if (vehicle == null)
                return BadRequest();
            var domainVehicle = vehicle.ToDomainModel();
            _vehicleRepository.Add(domainVehicle);
            return CreatedAtRoute("GetVehicle", new { id = vehicle.Id }, VehicleApiModel.FromDomainModel(domainVehicle));
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
    }
}
