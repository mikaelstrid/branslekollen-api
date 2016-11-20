using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models;

namespace API.Controllers
{
    public interface IVehicleRepository
    {
        void Add(Vehicle vehicle);
        Task<IEnumerable<Vehicle>> GetAll();
        Task<Vehicle> Find(string id);
        void Remove(string id);
        void Update(Vehicle vehicle);
    }
}