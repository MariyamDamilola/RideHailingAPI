using Ride_Hailing_API.DTO.Request;
using Ride_Hailing_API.DTO.Response;

namespace Ride_Hailing_API.Services.Interfaces;

public interface IUserService
{
    Task<ApiResponse> GetProfileAsync(int userId);

    Task<ApiResponse> DeactivateUserAsync(int userId, DeactivateUserRequestDto request);
}