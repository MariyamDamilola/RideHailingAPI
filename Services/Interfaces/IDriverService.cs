using Ride_Hailing_API.DTO.Request;
using Ride_Hailing_API.DTO.Response;

namespace Ride_Hailing_API.Services.Interfaces;

public interface IDriverService
{
    Task<ApiResponse> GetProfileAsync(int driverProfileId);
    
    Task<int?> GetDriverProfileIdByUserIdAsync(int userId);

    Task<ApiResponse> GetAssignedRidesAsync(int driverProfileId);

    Task<ApiResponse> GetCurrentRideAsync(int driverProfileId);

    Task<ApiResponse> SetAvailabilityAsync(int driverProfileId, bool isAvailable);
    
    Task<ApiResponse> AddVehicleAsync(int driverProfileId, CreateVehicleRequestDto request);
}