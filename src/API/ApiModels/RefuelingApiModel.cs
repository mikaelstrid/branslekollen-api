using System;
using API.Models;

namespace API.ApiModels
{
    public class RefuelingApiModel
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public bool MissedRefuelings { get; set; }
        public double NumberOfLiters { get; set; }
        public double PricePerLiter { get; set; }
        public int Odometer { get; set; }
        public bool FullTank { get; set; }

        public static RefuelingApiModel FromDomainModel(Refueling domainRefueling)
        {
            return new RefuelingApiModel
            {
                Id = domainRefueling.Id,
                Date = domainRefueling.Date,
                MissedRefuelings = domainRefueling.MissedRefuelings,
                NumberOfLiters = domainRefueling.NumberOfLiters,
                PricePerLiter = domainRefueling.PricePerLiter,
                Odometer = domainRefueling.Odometer,
                FullTank = domainRefueling.FullTank
            };
        }

        public Refueling ToDomainModel()
        {
            return new Refueling
            {
                Id = Id,
                Date = Date,
                MissedRefuelings = MissedRefuelings,
                NumberOfLiters = NumberOfLiters,
                PricePerLiter = PricePerLiter,
                Odometer = Odometer,
                FullTank = FullTank
            };
        }
    }
}