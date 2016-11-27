using System;
using System.Diagnostics.Contracts;
using API.Models;

namespace API.ApiModels
{
    public class RefuelingApiModel
    {
        public string Id { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime Date { get; set; }
        public bool MissedRefuelings { get; set; }
        public double NumberOfLiters { get; set; }
        public double PricePerLiter { get; set; }
        public int OdometerInKm { get; set; }
        public int? DistanceTravelledInKm { get; set; }
        public bool FullTank { get; set; }

        public static RefuelingApiModel FromDomainModel(Refueling domainRefueling)
        {
            return new RefuelingApiModel
            {
                Id = domainRefueling.Id,
                CreationTime = domainRefueling.CreationTime,
                Date = domainRefueling.Date,
                MissedRefuelings = domainRefueling.MissedRefuelings,
                NumberOfLiters = domainRefueling.NumberOfLiters,
                PricePerLiter = domainRefueling.PricePerLiter,
                OdometerInKm = domainRefueling.OdometerInKm,
                DistanceTravelledInKm = domainRefueling.DistanceTravelledInKm,
                FullTank = domainRefueling.FullTank
            };
        }

        public Refueling ToDomainModel()
        {
            Contract.Requires(Date.TimeOfDay == TimeSpan.Zero);

            return new Refueling
            {
                Id = Id,
                CreationTime = CreationTime,
                Date = Date,
                MissedRefuelings = MissedRefuelings,
                NumberOfLiters = NumberOfLiters,
                PricePerLiter = PricePerLiter,
                OdometerInKm = OdometerInKm,
                DistanceTravelledInKm = DistanceTravelledInKm,
                FullTank = FullTank
            };
        }
    }
}