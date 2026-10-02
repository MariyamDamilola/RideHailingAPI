namespace Ride_Hailing_API.DTO.Request;

public class CreateRideRequestDto
{
    public string? PickupAddress { get; set; }
    public string? DestinationAddress { get; set; }
}