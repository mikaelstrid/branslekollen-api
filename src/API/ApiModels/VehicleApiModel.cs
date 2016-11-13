using API.Models;

namespace API.ApiModels
{
    public class VehicleApiModel
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public static VehicleApiModel FromDomainModel(Vehicle vehicle)
        {
            return new VehicleApiModel {Id = vehicle.Id, Name = vehicle.Name};
        }

        public Vehicle ToDomainModel()
        {
            return new Vehicle { Id = Id, Name = Name };
        }
    }
}