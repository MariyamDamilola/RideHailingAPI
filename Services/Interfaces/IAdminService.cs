using Ride_Hailing_API.DTO.Request;
using Ride_Hailing_API.DTO.Response;

namespace Ride_Hailing_API.Services.Interfaces;

public interface IAdminService
{
    Task<ApiResponse> GetUsersAsync();

    Task<ApiResponse> GetUserByIdAsync(int userId);

    Task<ApiResponse> GetPendingDriversAsync();

    Task<ApiResponse> ApproveDriverAsync(int driverProfileId);

    Task<ApiResponse> RejectDriverAsync(int driverProfileId, RejectDriverRequestDto request);

    Task<ApiResponse> GetRidesAsync();

    Task<ApiResponse> GetAuditLogsAsync();
}