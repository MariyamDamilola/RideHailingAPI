namespace Ride_Hailing_API.DTO.Request;

public class ChangePasswordRequestDto
{
     public string? CurrentPassword { get; set; }
     public string? NewPassword { get; set; }
}