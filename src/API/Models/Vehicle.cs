using System.Collections.Generic;

namespace API.Models
{
    public class Vehicle
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Fuel Fuel { get; set; }
        public List<Refueling> Refuelings { get; set; } = new List<Refueling>();
    }

    public enum Fuel
    {
        Petrol = 0,
        Diesel = 1,
        E85 = 2
    }
}
