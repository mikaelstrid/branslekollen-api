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
        public int Odometer { get; set; }
        public bool FullTank { get; set; }
    }
}