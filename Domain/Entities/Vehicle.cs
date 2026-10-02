namespace Ride_Hailing_API.Domain.Entities;

public class Vehicle
{
    public int Id { get; set; }
    public int DriverProfileId { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public string Color { get; set; }
    public int Year { get; set; }
    public string LicensePlate { get; set; }
    public DriverProfile? DriverProfile { get; set; }
}