using Ride_Hailing_API.Domain.Entities;

namespace Ride_Hailing_API.Repository.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(int userId);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByPhoneNumberAsync(string phoneNumber);
    Task UpdateUserAsync(User user);
    Task DeactivateUserAsync(User user);

    Task SaveChangesAsync();
}