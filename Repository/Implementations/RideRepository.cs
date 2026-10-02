using Microsoft.EntityFrameworkCore;
using Ride_Hailing_API.Data;
using Ride_Hailing_API.Domain.Entities;
using Ride_Hailing_API.Repository.Interfaces;

namespace Ride_Hailing_API.Repository.Implementations;

public class RideRepository : IRideRepository
{
    private readonly AppDbContext _context;

    public RideRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Ride> CreateRideAsync(Ride ride)
    {
        await _context.Rides.AddAsync(ride);
        return ride;
    }

    public async Task<Ride?> GetRideByIdAsync(int rideId)
    {
        return await _context.Rides
            .Include(r => r.Passenger)
            .Include(r => r.DriverProfile)
            .ThenInclude(d => d.User)
            .FirstOrDefaultAsync(r => r.Id == rideId);
    }

    public async Task<Ride?> GetRideByReferenceAsync(string rideReference)
    {
        return await _context.Rides
            .Include(r => r.Passenger)
            .Include(r => r.DriverProfile)
            .FirstOrDefaultAsync(r => r.RideReference == rideReference);
    }

    public async Task<IEnumerable<Ride>> GetPassengerRidesAsync(int passengerId)
    {
        return await _context.Rides
            .Where(r => r.PassengerId == passengerId)
            .Include(r => r.DriverProfile)
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ride>> GetDriverRidesAsync(int driverProfileId)
    {
        return await _context.Rides
            .Where(r => r.DriverProfileId == driverProfileId)
            .Include(r => r.Passenger)
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<DriverProfile>> GetAvailableDriversAsync()
    {
        return await _context.DriverProfiles
            .Where(d => d.IsAvailable &&
                        d.Status == Domain.Enum.ApprovalStatus.Approved)
            .Include(d => d.User)
            .Include(d => d.Vehicles)
            .ToListAsync();
    }


    public async Task UpdateRideAsync(Ride ride)
    {
        _context.Rides.Update(ride);
    }


    public async Task<RideStatusHistory> CreateStatusHistoryAsync(
        RideStatusHistory history)
    {
        await _context.RideStatusHistories.AddAsync(history);
        return history;
    }

    public async Task<IEnumerable<RideStatusHistory>> GetRideStatusHistoryAsync(
        int rideId)
    {
        return await _context.RideStatusHistories
            .Where(h => h.RideId == rideId)
            .Include(h => h.ChangedByUser)
            .OrderBy(h => h.CreatedAt)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    } 
}