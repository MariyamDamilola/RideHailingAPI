using Ride_Hailing_API.DTO.Request;
using Ride_Hailing_API.DTO.Response;

namespace Ride_Hailing_API.Services.Interfaces;

public interface IAuthService
{
    Task<ApiResponse> RegisterAsync(RegisterRequestDto request);
    
    Task<ApiResponse> VerifyEmailAsync(VerifyEmailRequestDto request);
    Task<ApiResponse> ResendEmailOtpAsync(string email);
    Task<ApiResponse> ResendPasswordResetOtpAsync(string email);
    
    Task<ApiResponse> VerifyPhoneAsync(VerifyPhoneRequestDto request);
    Task<ApiResponse> ResendPhoneOtpAsync(string phoneNumber);
    
    Task<ApiResponse> LoginAsync(LoginRequestDto request);
    
    Task<ApiResponse> ForgotPasswordAsync(ForgetPasswordRequestDto request);
    
    Task<ApiResponse> ResetPasswordAsync(ResetPasswordRequestDto request);
    
    Task<ApiResponse> ChangePasswordAsync(int userId, ChangePasswordRequestDto request);
    
}