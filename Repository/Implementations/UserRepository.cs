
using Microsoft.EntityFrameworkCore;
using Ride_Hailing_API.Data;
using Ride_Hailing_API.Domain.Entities;
using Ride_Hailing_API.Repository.Interfaces;

namespace Ride_Hailing_API.Repository.Implementations;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId);
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

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
    }

    public async Task DeactivateUserAsync(User user)
    {
        user.IsActive = false;
        _context.Users.Update(user);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}