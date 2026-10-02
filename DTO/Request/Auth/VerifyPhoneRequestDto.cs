namespace Ride_Hailing_API.DTO.Request;

public class VerifyPhoneRequestDto
{
    public string? PhoneNumber { get; set; }
    public string? Code { get; set; }
}