using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace API.Models
{
    public interface IVehicle
    {
        string Id { get; set; }
        string Name { get; set; }
        Fuel Fuel { get; set; }
        List<Refueling> Refuelings { get; set; }
        double? CalculateFuelConsumption(DateTime startDate, DateTime endDate);
    }

    public class Vehicle : IVehicle
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Fuel Fuel { get; set; }
        public List<Refueling> Refuelings { get; set; } = new List<Refueling>();

        public double? CalculateFuelConsumption(DateTime startDate, DateTime endDate)
        {
            Contract.Requires(startDate.TimeOfDay == TimeSpan.Zero);
            Contract.Requires(endDate.TimeOfDay == TimeSpan.Zero); 

            if (Refuelings == null || Refuelings.Count < 2) return null;

            var refuelings = Refuelings
                .Where(r => startDate <= r.Date && r.Date <= endDate)
                .Where(r => r.FuelConsumptionInLitersPerKm.HasValue)
                .ToArray();

            return refuelings.Any() ? refuelings.Average(r => r.FuelConsumptionInLitersPerKm.Value) : (double?) null;
        }
    }

    public enum Fuel
    {
        Petrol = 0,
        Diesel = 1,
        E85 = 2
    }
}
