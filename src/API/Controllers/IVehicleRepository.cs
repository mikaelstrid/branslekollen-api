using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models;

namespace API.Controllers
{
    public interface IVehicleRepository
    {
        void Add(IVehicle vehicle);
        Task<IEnumerable<IVehicle>> GetAll();
        Task<IVehicle> Find(string id);
        void Remove(string id);
        void Update(IVehicle vehicle);
    }
}