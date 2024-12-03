namespace VehicleRentalApp.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string? VehicleName { get; set; }
        public string? PlateNumber { get; set; }
        public double ActiveWorkingHours { get; set; }
        public double MaintenanceHours { get; set; }
        public double IdleHours { get; set; } // Boşta bekleme süresi
    }
}
