using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using API.Models;

namespace API.DAL
{
    public interface IVehicleRepository
    {
        void Add(Vehicle vehicle);
        IEnumerable<Vehicle> GetAll();
        Vehicle Find(string id);
        Vehicle Remove(string id);
        void Update(Vehicle item);
    }

    public class VehicleRepository : IVehicleRepository
    {
        // ReSharper disable once InconsistentNaming
        private static readonly ConcurrentDictionary<string, Vehicle> _vehicles = new ConcurrentDictionary<string, Vehicle>();

        public VehicleRepository()
        {
            Add(new Vehicle { Name = "Vehicle 1" });
        }

        public void Add(Vehicle vehicle)
        {
            vehicle.Id = Guid.NewGuid().ToString();
            _vehicles[vehicle.Id] = vehicle;

        }

        public IEnumerable<Vehicle> GetAll()
        {
            return _vehicles.Values;

        }

        public Vehicle Find(string id)
        {
            Vehicle item;
            _vehicles.TryGetValue(id, out item);
            return item;

        }

        public Vehicle Remove(string id)
        {
            Vehicle item;
            _vehicles.TryRemove(id, out item);
            return item;

        }

        public void Update(Vehicle item)
        {
            _vehicles[item.Id] = item;
        }
    }
}
