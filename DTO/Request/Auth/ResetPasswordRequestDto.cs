namespace Ride_Hailing_API.DTO.Request;

public class ResetPasswordRequestDto
{
    public string? Email { get; set; }
    public string? Code { get; set; }
    public string? NewPassword { get; set; }
}