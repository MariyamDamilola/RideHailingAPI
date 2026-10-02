namespace Ride_Hailing_API.DTO.Response;

public class ApiResponse
{
    public string? ResponseCode { get; set; } 
    public string? ResponseMessage { get; set; } 
    public object? Data { get; set; } = null;
}