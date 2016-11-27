using System;

namespace API.ApiModels
{
    public class FuelConsumptionStatisticsApiModel : IEquatable<FuelConsumptionStatisticsApiModel>
    {
        public string VehicleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double? FuelConsumptionInLitersPerKm { get; set; }

        public FuelConsumptionStatisticsApiModel()
        {
        }

        public FuelConsumptionStatisticsApiModel(string vehicleId, DateTime startDate, DateTime endDate, double? fuelConsumptionInLitersPerKm)
        {
            VehicleId = vehicleId;
            StartDate = startDate;
            EndDate = endDate;
            FuelConsumptionInLitersPerKm = fuelConsumptionInLitersPerKm;
        }

        public bool Equals(FuelConsumptionStatisticsApiModel other)
        {
            return
                VehicleId == other.VehicleId
                && StartDate == other.StartDate
                && EndDate == other.EndDate
                && (
                    !FuelConsumptionInLitersPerKm.HasValue && !other.FuelConsumptionInLitersPerKm.HasValue
                    || FuelConsumptionInLitersPerKm.HasValue && other.FuelConsumptionInLitersPerKm.HasValue && Math.Abs(FuelConsumptionInLitersPerKm.Value - other.FuelConsumptionInLitersPerKm.Value) < 0.0001
                    );
        }

        public override string ToString()
        {
            return $"{VehicleId};{StartDate.ToString("yyyy-MM-dd")};{EndDate.ToString("yyyy-MM-dd")};{(FuelConsumptionInLitersPerKm.HasValue ? FuelConsumptionInLitersPerKm.Value.ToString("N4") : "null")}";
        }
    }
}
