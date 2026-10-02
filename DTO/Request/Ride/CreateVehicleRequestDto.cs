namespace Ride_Hailing_API.DTO.Request;

public class CreateVehicleRequestDto
{
    public string? Make { get; set; }
    public string? Model { get; set; }
    public string? Color { get; set; }
    public int Year { get; set; }
    public string? LicensePlate { get; set; }
}