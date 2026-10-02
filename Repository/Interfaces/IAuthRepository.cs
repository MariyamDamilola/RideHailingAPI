using Ride_Hailing_API.Domain.Entities;

namespace Ride_Hailing_API.Repository.Interfaces;

public interface IAuthRepository
{
    Task<User?> GetUserByIdAsync(int userId);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByPhoneNumberAsync(string phoneNumber);
    Task<User> CreateUserAsync(User user);
    Task<DriverProfile> CreateDriverProfileAsync(DriverProfile driverProfile);
    
    Task<EmailOtp?> GetActiveEmailOtpAsync(int userId);
    Task<EmailOtp> CreateEmailOtpAsync(EmailOtp otp);
    Task UpdateEmailOtpAsync(EmailOtp otp);

    
    Task<PhoneOtp?> GetActivePhoneOtpAsync(int userId);
    Task<PhoneOtp> CreatePhoneOtpAsync(PhoneOtp otp);
    Task UpdatePhoneOtpAsync(PhoneOtp otp);

   
    Task<PasswordResetOtp?> GetActivePasswordResetOtpAsync(int userId);
    Task<PasswordResetOtp> CreatePasswordResetOtpAsync(PasswordResetOtp otp);
    Task UpdatePasswordResetOtpAsync(PasswordResetOtp otp);
    
    Task UpdateUserAsync(User user);
    Task SaveChangesAsync();
}