using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Ride_Hailing_API.Domain;
using Ride_Hailing_API.Domain.Entities;
using Ride_Hailing_API.Domain.Enum;
using Ride_Hailing_API.DTO.Request;
using Ride_Hailing_API.DTO.Response;
using Ride_Hailing_API.Repository.Interfaces;
using Ride_Hailing_API.Services.Interfaces;
using RideHailingAPI.Helpers;

namespace Ride_Hailing_API.Services.Implementation;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

     public AuthService(
        IAuthRepository authRepository,
        IAuditLogRepository auditLogRepository,
        IEmailService emailService,
        ISmsService smsService,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _authRepository = authRepository;
        _auditLogRepository = auditLogRepository;
        _emailService = emailService;
        _smsService = smsService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ApiResponse> RegisterAsync(RegisterRequestDto request)
    {
        try
        {
            var existingEmail = await _authRepository
                .GetUserByEmailAsync(request.Email!);

            if (existingEmail != null)
            {
                return new ApiResponse
                {
                    ResponseCode = "400",
                    ResponseMessage = "Email already exists"
                };
            }

            var existingPhone = await _authRepository
                .GetUserByPhoneNumberAsync(request.PhoneNumber!);

            if (existingPhone != null)
            {
                return new ApiResponse
                {
                    ResponseCode = "400",
                    ResponseMessage = "Phone number already exists"
                };
            }

            if (!Enum.TryParse<UserRole>(
                    request.Role,
                    true,
                    out var role))
            {
                return new ApiResponse
                {
                    ResponseCode = "400",
                    ResponseMessage = "Invalid role"
                };
            }

            var user = new User
            {
                FullName = request.FullName!,
                Email = request.Email!,
                PhoneNumber = request.PhoneNumber!,
                PasswordHash = Utils.EncryptPassword(request.Password!),
                Role = role,
                IsEmailVerified = false,
                IsPhoneNumberVerified = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdUser = await _authRepository
                .CreateUserAsync(user);

            await _authRepository.SaveChangesAsync();

            if (createdUser.Role == UserRole.Driver)
            {
                var driverProfile = new DriverProfile
                {
                    UserId = createdUser.Id,
                    IsAvailable = false,
                    Status = ApprovalStatus.Pending,
                    ApprovalReason = null,
                    ApprovedAt = null
                };

                await _authRepository
                    .CreateDriverProfileAsync(driverProfile);

                await _authRepository.SaveChangesAsync();
            }

            // Create OTPs for a new user
            var emailOtp = new EmailOtp
            {
                UserId = createdUser.Id,
                Code = Utils.GenerateOtp(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            var phoneOtp = new PhoneOtp
            {
                UserId = createdUser.Id,
                Code = Utils.GenerateOtp(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            await _authRepository.CreateEmailOtpAsync(emailOtp);
            await _authRepository.CreatePhoneOtpAsync(phoneOtp);

            await _authRepository.SaveChangesAsync();

            _logger.LogInformation(
                $"Successfully registered user {createdUser.Id} - {createdUser.FullName}");

            await _emailService.SendRegistrationOtpEmailAsync(
                createdUser.Email,
                createdUser.FullName,
                emailOtp.Code);

            await _smsService.SendSmsAsync(
                createdUser.PhoneNumber,
                SmsUtils.GetRegistrationSms(
                    createdUser.FullName,
                    createdUser.Id,
                    phoneOtp.Code));

            await _auditLogRepository.CreateAsync(new AuditLog
            {
                UserId = createdUser.Id,
                UserRole = createdUser.Role,
                Action = "User Registration",
                Status = AuditStatus.Success,
                EntityType = "User",
                EntityId = createdUser.Id,
                Description = $"User {createdUser.FullName} registered successfully",
                CreatedAt = DateTime.UtcNow
            });

            await _auditLogRepository.SaveChangesAsync();

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage =
                    "Registration successful. Verification codes have been sent."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred during registration for {request.Email}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Error occurred during registration"
            };
        }
    }


    public async Task<ApiResponse> VerifyEmailAsync(
       VerifyEmailRequestDto request)
   {
       try
       {
           var user = await _authRepository
               .GetUserByEmailAsync(request.Email!);
   
           if (user == null)
           {
               _logger.LogWarning(
                   $"Email verification failed: User not found for {request.Email}");
   
               return new ApiResponse
               {
                   ResponseCode = "400",
                   ResponseMessage = "Invalid verification details"
               };
           }
   
           var otp = await _authRepository
               .GetActiveEmailOtpAsync(user.Id);
   
           if (otp == null ||
               otp.IsUsed ||
               otp.ExpiresAt < DateTime.UtcNow ||
               otp.Code != request.Code)
           {
               _logger.LogWarning(
                   $"Email verification failed for user {user.Id}");
   
               await _auditLogRepository.CreateAsync(new AuditLog
               {
                   UserId = user.Id,
                   UserRole = user.Role,
                   Action = "Email Verification",
                   Status = AuditStatus.Failure,
                   EntityType = "EmailOtp",
                   EntityId = otp?.Id,
                   Description = "Invalid or expired email verification OTP",
                   CreatedAt = DateTime.UtcNow
               });
   
               await _auditLogRepository.SaveChangesAsync();
   
               return new ApiResponse
               {
                   ResponseCode = "400",
                   ResponseMessage = "Invalid or expired verification code"
               };
           }
   
           otp.IsUsed = true;
           otp.VerifiedAt = DateTime.UtcNow;
   
           user.IsEmailVerified = true;
           user.UpdatedAt = DateTime.UtcNow;
   
           await _authRepository.UpdateEmailOtpAsync(otp);
           await _authRepository.UpdateUserAsync(user);
           await _authRepository.SaveChangesAsync();
   
           _logger.LogInformation(
               $"Email successfully verified for user {user.Id}");
   
           await _auditLogRepository.CreateAsync(new AuditLog
           {
               UserId = user.Id,
               UserRole = user.Role,
               Action = "Email Verification",
               Status = AuditStatus.Success,
               EntityType = "User",
               EntityId = user.Id,
               Description = "User email verified successfully",
               CreatedAt = DateTime.UtcNow
           });
   
           await _auditLogRepository.SaveChangesAsync();
   
           return new ApiResponse
           {
               ResponseCode = "200",
               ResponseMessage = "Email verified successfully"
           };
       }
       catch (Exception ex)
       {
           _logger.LogError(
               ex,
               $"Error occurred during email verification for {request.Email}");
   
           return new ApiResponse
           {
               ResponseCode = "500",
               ResponseMessage = "Error occurred during email verification"
           };
       }
   }

   public async Task<ApiResponse> VerifyPhoneAsync(
       VerifyPhoneRequestDto request)
   {
       try
       {
           var user = await _authRepository
               .GetUserByPhoneNumberAsync(request.PhoneNumber!);

           if (user == null)
           {
               _logger.LogWarning(
                   $"Phone verification failed: User not found for {request.PhoneNumber}");

               return new ApiResponse
               {
                   ResponseCode = "400",
                   ResponseMessage = "Invalid verification details"
               };
           }

           var otp = await _authRepository
               .GetActivePhoneOtpAsync(user.Id);

           if (otp == null ||
               otp.IsUsed ||
               otp.ExpiresAt < DateTime.UtcNow ||
               otp.Code != request.Code)
           {
               _logger.LogWarning(
                   $"Phone verification failed for user {user.Id}");

               await _auditLogRepository.CreateAsync(new AuditLog
               {
                   UserId = user.Id,
                   UserRole = user.Role,
                   Action = "Phone Verification",
                   Status = AuditStatus.Failure,
                   EntityType = "PhoneOtp",
                   EntityId = otp?.Id,
                   Description = "Invalid or expired phone verification OTP",
                   CreatedAt = DateTime.UtcNow
               });

               await _auditLogRepository.SaveChangesAsync();

               return new ApiResponse
               {
                   ResponseCode = "400",
                   ResponseMessage = "Invalid or expired verification code"
               };
           }

           otp.IsUsed = true;
           otp.VerifiedAt = DateTime.UtcNow;

           user.IsPhoneNumberVerified = true;
           user.UpdatedAt = DateTime.UtcNow;

           await _authRepository.UpdatePhoneOtpAsync(otp);
           await _authRepository.UpdateUserAsync(user);
           await _authRepository.SaveChangesAsync();

           _logger.LogInformation(
               $"Phone number successfully verified for user {user.Id}");

           if (user.IsEmailVerified && user.IsPhoneNumberVerified)
           {
               await _emailService.SendAccountVerifiedEmailAsync(
                   user.Email,
                   user.FullName);

               await _smsService.SendSmsAsync(
                   user.PhoneNumber,
                   SmsUtils.GetVerificationSuccessSms(
                       user.FullName,
                       user.Id));

               _logger.LogInformation(
                   $"User {user.Id} has completed email and phone verification.");
           }

           await _auditLogRepository.CreateAsync(new AuditLog
           {
               UserId = user.Id,
               UserRole = user.Role,
               Action = "Phone Verification",
               Status = AuditStatus.Success,
               EntityType = "User",
               EntityId = user.Id,
               Description = user.IsEmailVerified
                   ? "User phone number verified successfully. Account fully verified."
                   : "User phone number verified successfully.",
               CreatedAt = DateTime.UtcNow
           });

           await _auditLogRepository.SaveChangesAsync();

           return new ApiResponse
           {
               ResponseCode = "200",
               ResponseMessage = user.IsEmailVerified
                   ? "Phone number verified successfully. Your account is now fully verified."
                   : "Phone number verified successfully"
           };
       }
       catch (Exception ex)
       {
           _logger.LogError(
               ex,
               $"Error occurred during phone verification for {request.PhoneNumber}");

           return new ApiResponse
           {
               ResponseCode = "500",
               ResponseMessage = "Error occurred during phone verification"
           };
       }
   }


   public async Task<ApiResponse> LoginAsync(LoginRequestDto request)
    {
        try
        {
            var user = await _authRepository
                .GetUserByEmailAsync(request.Email!);

            if (user == null ||
                !Utils.VerifyPassword(
                    request.Password!,
                    user.PasswordHash))
            {
                _logger.LogWarning(
                    $"Login failed for {request.Email}");

                return new ApiResponse
                {
                    ResponseCode = "401",
                    ResponseMessage = "Invalid email or password"
                };
            }

            if (!user.IsActive)
            {
                _logger.LogWarning(
                    $"Login rejected: User {user.Id} is inactive");

                return new ApiResponse
                {
                    ResponseCode = "403",
                    ResponseMessage = "Account is inactive"
                };
            }

            if (!user.IsEmailVerified ||
                !user.IsPhoneNumberVerified)
            {
                _logger.LogWarning(
                    $"Login rejected: User {user.Id} is not fully verified");

                return new ApiResponse
                {
                    ResponseCode = "403",
                    ResponseMessage =
                        "Please verify your email and phone number before logging in"
                };
            }

            var token = GenerateJwt(user);

            _logger.LogInformation(
                $"Successful login for user {user.Id}");

            await _auditLogRepository.CreateAsync(new AuditLog
            {
                UserId = user.Id,
                UserRole = user.Role,
                Action = "Login",
                Status = AuditStatus.Success,
                EntityType = "User",
                EntityId = user.Id,
                Description = "User logged in successfully",
                CreatedAt = DateTime.UtcNow
            });

            await _auditLogRepository.SaveChangesAsync();

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Login successful",
                Data = token
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred during login for {request.Email}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Error occurred during login"
            };
        }
    }


    public async Task<ApiResponse> ForgotPasswordAsync(
        ForgetPasswordRequestDto request)
    {
        try
        {
            var user = await _authRepository.GetUserByEmailAsync(request.Email!);

            if (user == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "200",
                    ResponseMessage = "If the email exists, a password reset OTP has been sent."
                };
            }

            var existingOtp = await _authRepository
                .GetActivePasswordResetOtpAsync(user.Id);

            if (existingOtp != null)
            {
                existingOtp.IsUsed = true;
                await _authRepository.UpdatePasswordResetOtpAsync(existingOtp);
            }

            var otp = new PasswordResetOtp
            {
                UserId = user.Id,
                Code = Utils.GenerateOtp(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            await _authRepository.CreatePasswordResetOtpAsync(otp);

            await _authRepository.SaveChangesAsync();

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage =
                    "If the email exists, a password reset code has been sent."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred during password reset request");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage =
                    "Error occurred while processing password reset request"
            };
        }
    }


    public async Task<ApiResponse> ResetPasswordAsync(
        ResetPasswordRequestDto request)
    {
        try
        {
            var user = await _authRepository
                .GetUserByEmailAsync(request.Email!);

            if (user == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "400",
                    ResponseMessage = "Invalid password reset details"
                };
            }

            var otp = await _authRepository
                .GetActivePasswordResetOtpAsync(user.Id);

            if (otp == null ||
                otp.IsUsed ||
                otp.ExpiresAt < DateTime.UtcNow ||
                otp.Code != request.Code)
            {
                _logger.LogWarning(
                    $"Password reset failed for user {user.Id}");

                await _auditLogRepository.CreateAsync(new AuditLog
                {
                    UserId = user.Id,
                    UserRole = user.Role,
                    Action = "Password Reset",
                    Status = AuditStatus.Failure,
                    EntityType = "PasswordResetOtp",
                    EntityId = otp?.Id,
                    Description = "Invalid or expired password reset OTP",
                    CreatedAt = DateTime.UtcNow
                });

                await _auditLogRepository.SaveChangesAsync();

                return new ApiResponse
                {
                    ResponseCode = "400",
                    ResponseMessage = "Invalid or expired reset code"
                };
            }

            user.PasswordHash = Utils.EncryptPassword(
                request.NewPassword!);

            user.UpdatedAt = DateTime.UtcNow;
            otp.IsUsed = true;

            await _authRepository.UpdateUserAsync(user);
            await _authRepository.UpdatePasswordResetOtpAsync(otp);
            await _authRepository.SaveChangesAsync();

            _logger.LogInformation(
                $"Password successfully reset for user {user.Id}");

            await _emailService.SendPasswordChangedEmailAsync(
                user.Email,
                user.FullName);

            await _smsService.SendSmsAsync(
                user.PhoneNumber,
                SmsUtils.GetPasswordChangedSms(user.FullName));

            await _auditLogRepository.CreateAsync(new AuditLog
            {
                UserId = user.Id,
                UserRole = user.Role,
                Action = "Password Reset",
                Status = AuditStatus.Success,
                EntityType = "User",
                EntityId = user.Id,
                Description = "User password reset successfully",
                CreatedAt = DateTime.UtcNow
            });

            await _auditLogRepository.SaveChangesAsync();

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Password reset successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred during password reset");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Error occurred during password reset"
            };
        }
    }


    public async Task<ApiResponse> ChangePasswordAsync(
        int userId,
        ChangePasswordRequestDto request)
    {
        try
        {
            var user = await _authRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "User not found"
                };
            }

            if (!Utils.VerifyPassword(
                    request.CurrentPassword!,
                    user.PasswordHash))
            {
                _logger.LogWarning(
                    $"Change password failed for user {userId}");

                await _auditLogRepository.CreateAsync(new AuditLog
                {
                    UserId = userId,
                    UserRole = user.Role,
                    Action = "Change Password",
                    Status = AuditStatus.Failure,
                    EntityType = "User",
                    EntityId = userId,
                    Description = "Incorrect current password",
                    CreatedAt = DateTime.UtcNow
                });

                await _auditLogRepository.SaveChangesAsync();

                return new ApiResponse
                {
                    ResponseCode = "400",
                    ResponseMessage = "Current password is incorrect"
                };
            }

            user.PasswordHash = Utils.EncryptPassword(
                request.NewPassword!);

            user.UpdatedAt = DateTime.UtcNow;

            await _authRepository.UpdateUserAsync(user);
            await _authRepository.SaveChangesAsync();

            _logger.LogInformation(
                $"Password changed successfully for user {userId}");

            await _emailService.SendPasswordChangedEmailAsync(
                user.Email,
                user.FullName);

            await _smsService.SendSmsAsync(
                user.PhoneNumber,
                SmsUtils.GetPasswordChangedSms(user.FullName));

            await _auditLogRepository.CreateAsync(new AuditLog
            {
                UserId = user.Id,
                UserRole = user.Role,
                Action = "Change Password",
                Status = AuditStatus.Success,
                EntityType = "User",
                EntityId = user.Id,
                Description = "User changed password successfully",
                CreatedAt = DateTime.UtcNow
            });

            await _auditLogRepository.SaveChangesAsync();

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Password changed successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while changing password for user {userId}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Error occurred while changing password"
            };
        }
    }
    
    public async Task<ApiResponse> ResendEmailOtpAsync(string email)
    {
        try
        {
            var user = await _authRepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "User not found"
                };
            }

            if (user.IsEmailVerified)
            {
                return new ApiResponse
                {
                    ResponseCode = "400",
                    ResponseMessage = "Email is already verified"
                };
            }

            var existingOtp = await _authRepository
                .GetActiveEmailOtpAsync(user.Id);

            if (existingOtp != null)
            {
                existingOtp.IsUsed = true;
                await _authRepository.UpdateEmailOtpAsync(existingOtp);
            }

            var newOtp = new EmailOtp
            {
                UserId = user.Id,
                Code = Utils.GenerateOtp(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            await _authRepository.CreateEmailOtpAsync(newOtp);
            await _authRepository.SaveChangesAsync();

            var firstName = user.FullName.Split(' ')[0];

            await _emailService.SendResendOtpEmailAsync(
                user.Email,
                firstName,
                newOtp.Code);

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "A new email verification OTP has been sent"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while resending email OTP for {email}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "An error occurred while resending email OTP"
            };
        }
    }
    
    public async Task<ApiResponse> ResendPhoneOtpAsync(string phoneNumber)
    {
        try
        {
            var user = await _authRepository
                .GetUserByPhoneNumberAsync(phoneNumber);

            if (user == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "User not found"
                };
            }

            if (user.IsPhoneNumberVerified)
            {
                return new ApiResponse
                {
                    ResponseCode = "400",
                    ResponseMessage = "Phone number is already verified"
                };
            }

            var existingOtp = await _authRepository
                .GetActivePhoneOtpAsync(user.Id);

            if (existingOtp != null)
            {
                existingOtp.IsUsed = true;
                await _authRepository.UpdatePhoneOtpAsync(existingOtp);
            }

            var newOtp = new PhoneOtp
            {
                UserId = user.Id,
                Code = Utils.GenerateOtp(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            await _authRepository.CreatePhoneOtpAsync(newOtp);
            await _authRepository.SaveChangesAsync();

            var firstName = user.FullName.Split(' ')[0];

            await _smsService.SendSmsAsync(
                user.PhoneNumber,
                SmsUtils.GetResendOtpSms(
                    firstName,
                    newOtp.Code));

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "A new phone verification OTP has been sent"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while resending phone OTP for {phoneNumber}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "An error occurred while resending phone OTP"
            };
        }
    }
    
    public async Task<ApiResponse> ResendPasswordResetOtpAsync(string email)
    {
        try
        {
            var user = await _authRepository.GetUserByEmailAsync(email);

            // Do not reveal whether the email exists
            if (user == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "200",
                    ResponseMessage = "If the email exists, a new password reset OTP has been sent"
                };
            }

            var existingOtp = await _authRepository
                .GetActivePasswordResetOtpAsync(user.Id);

            if (existingOtp != null)
            {
                existingOtp.IsUsed = true;
                await _authRepository.UpdatePasswordResetOtpAsync(existingOtp);
            }

            var newOtp = new PasswordResetOtp
            {
                UserId = user.Id,
                Code = Utils.GenerateOtp(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            await _authRepository.CreatePasswordResetOtpAsync(newOtp);
            await _authRepository.SaveChangesAsync();

            var firstName = user.FullName.Split(' ')[0];

            await _emailService.SendPasswordResetOtpEmailAsync(
                user.Email,
                firstName,
                newOtp.Code);

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "If the email exists, a new password reset OTP has been sent"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while resending password reset OTP for {email}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "An error occurred while resending password reset OTP"
            };
        }
    }




    private object GenerateJwt(User user)
    {
        var expiryMinutes = int.Parse(
            _configuration["JWT:ExpirationInMinutes"]!
        );

        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var claims = new List<Claim>
        {
            new(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()
            ),
            new(
                ClaimTypes.Name,
                user.FullName
            ),
            new(
                ClaimTypes.Email,
                user.Email
            ),
            new(
                ClaimTypes.Role,
                user.Role.ToString()
            ),
            new(
                "phoneNumber",
                user.PhoneNumber
            )
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _configuration["JWT:Key"]!
            )
        );

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"]!,
            audience: _configuration["JWT:Audience"]!,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return new
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt
        };
    }
}

