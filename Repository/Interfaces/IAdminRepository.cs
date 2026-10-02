using Ride_Hailing_API.Domain.Entities;

namespace Ride_Hailing_API.Repository.Interfaces;

public interface IAdminRepository
{
    Task<IEnumerable<User>> GetUsersAsync();

    Task<User?> GetUserByIdAsync(int userId);

    Task<IEnumerable<DriverProfile>> GetPendingDriversAsync();

    Task<DriverProfile?> GetDriverProfileByIdAsync(int driverProfileId);

    Task<IEnumerable<Ride>> GetRidesAsync();

    Task<IEnumerable<AuditLog>> GetAuditLogsAsync();

    Task UpdateDriverProfileAsync(DriverProfile driverProfile);

    Task SaveChangesAsync();
}