using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ride_Hailing_API.DTO.Request;
using Ride_Hailing_API.Services.Interfaces;

namespace Ride_Hailing_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequestDto request)
    {
        return Ok(await _authService.RegisterAsync(request));
    }

    [HttpPost("verify-email")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyEmail(VerifyEmailRequestDto request)
    {
        return Ok(await _authService.VerifyEmailAsync(request));
    }
    
    [HttpPost("resend-email-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendEmailOtp(string email)
    {
        return Ok(await _authService.ResendEmailOtpAsync(email));
    }

    [HttpPost("verify-phone")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyPhone(
        VerifyPhoneRequestDto request)
    {
        return Ok(await _authService.VerifyPhoneAsync(request));
    }
    [HttpPost("resend-password-reset-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendPasswordResetOtp(string email)
    {
        return Ok(
            await _authService.ResendPasswordResetOtpAsync(email));
    }
    
    [HttpPost("resend-phone-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendPhoneOtp(string phoneNumber)
    {
        return Ok(
            await _authService.ResendPhoneOtpAsync(phoneNumber));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequestDto request)
    {
        return Ok(await _authService.LoginAsync(request));
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(
        ForgetPasswordRequestDto request)
    {
        return Ok(await _authService.ForgotPasswordAsync(request));
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(
        ResetPasswordRequestDto request)
    {
        return Ok(await _authService.ResetPasswordAsync(request));
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(
        ChangePasswordRequestDto request)
    {
        var userId = int.Parse(
            User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        return Ok(
            await _authService.ChangePasswordAsync(
                userId,
                request));
    }
}