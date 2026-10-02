using Microsoft.EntityFrameworkCore;
using Ride_Hailing_API.Data;
using Ride_Hailing_API.Domain.Entities;
using Ride_Hailing_API.Domain.Enum;
using Ride_Hailing_API.Repository.Interfaces;

namespace Ride_Hailing_API.Repository.Implementations;

public class AdminRepository : IAdminRepository
{
    private readonly AppDbContext _context;

    public AdminRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<IEnumerable<DriverProfile>> GetPendingDriversAsync()
    {
        return await _context.DriverProfiles
            .Include(d => d.User)
            .Include(d => d.Vehicles)
            .Where(d => d.Status == ApprovalStatus.Pending)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<DriverProfile?> GetDriverProfileByIdAsync(
        int driverProfileId)
    {
        return await _context.DriverProfiles
            .Include(d => d.User)
            .Include(d => d.Vehicles)
            .FirstOrDefaultAsync(d => d.Id == driverProfileId);
    }

    public async Task<IEnumerable<Ride>> GetRidesAsync()
    {
        return await _context.Rides
            .Include(r => r.Passenger)
            .Include(r => r.DriverProfile)
            .ThenInclude(d => d.User)
            .AsNoTracking()
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync()
    {
        return await _context.AuditLogs
            .Include(a => a.User)
            .AsNoTracking()
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateDriverProfileAsync(
        DriverProfile driverProfile)
    {
        _context.DriverProfiles.Update(driverProfile);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}