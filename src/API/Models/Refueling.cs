using System;

namespace API.Models
{
    public class Refueling
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public bool MissedRefuelings { get; set; }
        public double NumberOfLiters { get; set; }
        public double PricePerLiter { get; set; }
        public int OdometerInKm { get; set; }
        public int? DistanceTravelledInKm { get; set; }
        public bool FullTank { get; set; }
    }
}