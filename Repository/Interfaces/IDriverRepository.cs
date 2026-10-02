using Ride_Hailing_API.Domain.Entities;

namespace Ride_Hailing_API.Repository.Interfaces;

public interface IDriverRepository
{
    Task<DriverProfile?> GetDriverProfileByUserIdAsync(int userId);

    Task<DriverProfile?> GetDriverProfileByIdAsync(int driverProfileId);

    Task<IEnumerable<Ride>> GetAssignedRidesAsync(int driverProfileId);

    Task<Ride?> GetCurrentRideAsync(int driverProfileId);

    Task UpdateDriverProfileAsync(DriverProfile driverProfile);

    Task<Vehicle> AddVehicleAsync(Vehicle vehicle);

    Task SaveChangesAsync();
}