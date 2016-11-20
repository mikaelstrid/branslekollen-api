using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Controllers;
using API.Models;

namespace API.DAL
{
    public class MemoryVehicleRepository : IVehicleRepository
    {
        // ReSharper disable once InconsistentNaming
        private static readonly ConcurrentDictionary<string, Vehicle> _vehicles = new ConcurrentDictionary<string, Vehicle>();

        public MemoryVehicleRepository()
        {
            Add(new Vehicle { Name = "Vehicle 1" });
        }

        public void Add(Vehicle vehicle)
        {
            _vehicles[vehicle.Id] = vehicle;
        }

        public Task<IEnumerable<Vehicle>> GetAll()
        {
            var result = new List<Vehicle>();
            result.AddRange(_vehicles.Values);
            return Task.FromResult((IEnumerable<Vehicle>) result);
        }

        public Task<Vehicle> Find(string id)
        {
            Vehicle item;
            _vehicles.TryGetValue(id, out item);
            return Task.FromResult(item);
        }

        public void Remove(string id)
        {
            Vehicle item;
            _vehicles.TryRemove(id, out item);
        }

        public void Update(Vehicle vehicle)
        {
            _vehicles[vehicle.Id] = vehicle;
        }
    }
}
