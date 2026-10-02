namespace Ride_Hailing_API.DTO.Request;

public class VerifyEmailRequestDto
{
    public string? Email { get; set; }
    public string? Code { get; set; }
}