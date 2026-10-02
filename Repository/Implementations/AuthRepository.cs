using Microsoft.EntityFrameworkCore;
using Ride_Hailing_API.Data;
using Ride_Hailing_API.Domain.Entities;
using Ride_Hailing_API.Repository.Interfaces;

namespace Ride_Hailing_API.Repository.Implementations;

public class AuthRepository : IAuthRepository
{
     private readonly AppDbContext _context;

    public AuthRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
    } 
    
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<User?> GetUserByPhoneNumberAsync(string phoneNumber)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
    }


    public async Task<User> CreateUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        return user;
    }
 
    public async Task<DriverProfile> CreateDriverProfileAsync(DriverProfile driverProfile)
    {
        await _context.DriverProfiles.AddAsync(driverProfile);

        return driverProfile;
    }

    public async Task<EmailOtp?> GetActiveEmailOtpAsync(int userId)
    {
        return await _context.EmailOtps
            .Where(x => x.UserId == userId && !x.IsUsed)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<EmailOtp> CreateEmailOtpAsync(EmailOtp otp)
    {
        await _context.EmailOtps.AddAsync(otp);
        return otp;
    }

    public async Task UpdateEmailOtpAsync(EmailOtp otp)
    {
        _context.EmailOtps.Update(otp);
    }


    public async Task<PhoneOtp?> GetActivePhoneOtpAsync(int userId)
    {
        return await _context.PhoneOtps
            .Where(x => x.UserId == userId && !x.IsUsed)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<PhoneOtp> CreatePhoneOtpAsync(PhoneOtp otp)
    {
        await _context.PhoneOtps.AddAsync(otp);
        return otp;
    }

    public async Task UpdatePhoneOtpAsync(PhoneOtp otp)
    {
        _context.PhoneOtps.Update(otp);
    }


    public async Task<PasswordResetOtp?> GetActivePasswordResetOtpAsync(int userId)
    {
        return await _context.PasswordResetOtps
            .Where(x => x.UserId == userId && !x.IsUsed)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<PasswordResetOtp> CreatePasswordResetOtpAsync(
        PasswordResetOtp otp)
    {
        await _context.PasswordResetOtps.AddAsync(otp);
        return otp;
    }

    public async Task UpdatePasswordResetOtpAsync(
        PasswordResetOtp otp)
    {
        _context.PasswordResetOtps.Update(otp);
    }

    
    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}