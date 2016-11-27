using System;
using System.Collections.Generic;
using System.Linq;
using API.Models;

namespace API.ApiModels
{
    public class VehicleApiModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Fuel { get; set; }
        public List<RefuelingApiModel> Refuelings { get; set; } = new List<RefuelingApiModel>();

        public static VehicleApiModel FromDomainModel(IVehicle vehicle)
        {
            return new VehicleApiModel
            {
                Id = vehicle.Id,
                Name = vehicle.Name,
                Fuel = vehicle.Fuel.ToString(),
                Refuelings = vehicle.Refuelings.Select(RefuelingApiModel.FromDomainModel).ToList()
            };
        }

        public IVehicle ToDomainModel()
        {
            return new Vehicle { Id = Id, Name = Name, Fuel = ParseFuel(Fuel), Refuelings = Refuelings.Select(r => r.ToDomainModel()).ToList()};
        }

        public static Fuel ParseFuel(string input)
        {
            if (input.ToLowerInvariant() == "petrol") return Models.Fuel.Petrol;
            if (input.ToLowerInvariant() == "diesel") return Models.Fuel.Diesel;
            if (input.ToLowerInvariant() == "e85") return Models.Fuel.E85;
            throw new ArgumentException("Unknown fuel type", nameof(input));
        }
    }
}