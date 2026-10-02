using Microsoft.EntityFrameworkCore;
using Ride_Hailing_API.Data;
using Ride_Hailing_API.Domain.Entities;
using Ride_Hailing_API.Domain.Enum;
using Ride_Hailing_API.Repository.Interfaces;

namespace Ride_Hailing_API.Repository.Implementations;

public class DriverRepository : IDriverRepository
{
    private readonly AppDbContext _context;

    public DriverRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DriverProfile?> GetDriverProfileByUserIdAsync(
        int userId)
    {
        return await _context.DriverProfiles
            .Include(d => d.User)
            .Include(d => d.Vehicles)
            .FirstOrDefaultAsync(d => d.UserId == userId);
    }

    public async Task<DriverProfile?> GetDriverProfileByIdAsync(
        int driverProfileId)
    {
        return await _context.DriverProfiles
            .Include(d => d.User)
            .Include(d => d.Vehicles)
            .FirstOrDefaultAsync(d => d.Id == driverProfileId);
    }

    public async Task<IEnumerable<Ride>> GetAssignedRidesAsync(int driverProfileId)
    {
        return await _context.Rides
            .Include(r => r.Passenger)
            .Include(r => r.DriverProfile)
                .ThenInclude(d => d.User)
            .Where(r => r.DriverProfileId == driverProfileId)
            .OrderByDescending(r => r.RequestedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Ride?> GetCurrentRideAsync(int driverProfileId)
    {
        return await _context.Rides
            .Include(r => r.Passenger)
            .Include(r => r.DriverProfile)
                .ThenInclude(d => d.User)
            .FirstOrDefaultAsync(r =>
                r.DriverProfileId == driverProfileId &&
                r.Status != RideStatus.Completed &&
                r.Status != RideStatus.Cancelled);
    }

    public async Task UpdateDriverProfileAsync(DriverProfile driverProfile)
    {
        _context.DriverProfiles.Update(driverProfile);
        await Task.CompletedTask;
    }

    public async Task<Vehicle> AddVehicleAsync(Vehicle vehicle)
    {
        await _context.Vehicles.AddAsync(vehicle);
        return vehicle;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}