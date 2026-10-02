using Ride_Hailing_API.Domain.Entities;
using Ride_Hailing_API.Domain.Enum;
using Ride_Hailing_API.DTO.Request;
using Ride_Hailing_API.DTO.Response;
using Ride_Hailing_API.Repository.Interfaces;
using Ride_Hailing_API.Services.Interfaces;

namespace Ride_Hailing_API.Services.Implementation;

public class DriverService : IDriverService
{
    private readonly IDriverRepository _driverRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ILogger<DriverService> _logger;

    public DriverService(
        IDriverRepository driverRepository,
        IAuditLogRepository auditLogRepository,
        ILogger<DriverService> logger)
    {
        _driverRepository = driverRepository;
        _auditLogRepository = auditLogRepository;
        _logger = logger;
    }

    public async Task<ApiResponse> GetProfileAsync(int driverProfileId)
    {
        try
        {
            var driver = await _driverRepository
                .GetDriverProfileByIdAsync(driverProfileId);

            if (driver == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "Driver profile not found"
                };
            }

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Driver profile retrieved successfully",
                Data = new
                {
                    DriverProfileId = driver.Id,
                    driver.Status,
                    driver.IsAvailable,
                    driver.ApprovalReason,
                    driver.ApprovedAt,

                    User = new
                    {
                        driver.User.Id,
                        driver.User.FullName,
                        driver.User.Email,
                        driver.User.PhoneNumber
                    },

                    Vehicles = driver.Vehicles.Select(vehicle => new
                    {
                        vehicle.Id,
                        vehicle.Make,
                        vehicle.Model,
                        vehicle.Color,
                        vehicle.Year,
                        vehicle.LicensePlate
                    })
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while retrieving driver profile {driverProfileId}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Error occurred while retrieving driver profile"
            };
        }
    }

    public async Task<ApiResponse> GetAssignedRidesAsync(int driverProfileId)
    {
        try
        {
            var rides = await _driverRepository.GetAssignedRidesAsync(driverProfileId);

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Assigned rides retrieved successfully",
                Data = rides.Select(ride => new
                {
                    ride.Id,
                    ride.RideReference,
                    ride.PickupAddress,
                    ride.DestinationAddress,
                    ride.Status,
                    ride.RequestedAt,
                    ride.CompletedAt,
                    ride.CancelledAt,
                    ride.CancellationReason,

                    Passenger = ride.Passenger == null
                        ? null
                        : new
                        {
                            ride.Passenger.Id,
                            ride.Passenger.FullName,
                            ride.Passenger.PhoneNumber
                        }
                })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while retrieving assigned rides for driver {driverProfileId}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Error occurred while retrieving assigned rides"
            };
        }
    }

    public async Task<ApiResponse> GetCurrentRideAsync(
        int driverProfileId)
    {
        try
        {
            var ride = await _driverRepository
                .GetCurrentRideAsync(driverProfileId);

            if (ride == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "No current ride found"
                };
            }

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Current ride retrieved successfully",
                Data = new
                {
                    ride.Id,
                    ride.RideReference,
                    ride.PickupAddress,
                    ride.DestinationAddress,
                    ride.Status,
                    ride.RequestedAt,

                    Passenger = ride.Passenger == null
                        ? null
                        : new
                        {
                            ride.Passenger.Id,
                            ride.Passenger.FullName,
                            ride.Passenger.PhoneNumber
                        }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while retrieving current ride for driver {driverProfileId}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Error occurred while retrieving current ride"
            };
        }
    }

    public async Task<ApiResponse> SetAvailabilityAsync(int driverProfileId, bool isAvailable)
    {
        try
        {
            var driver = await _driverRepository
                .GetDriverProfileByIdAsync(driverProfileId);

            if (driver == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "Driver profile not found"
                };
            }

            if (driver.Status != ApprovalStatus.Approved)
            {
                return new ApiResponse
                {
                    ResponseCode = "403",
                    ResponseMessage = "Driver is not approved"
                };
            }

            driver.IsAvailable = isAvailable;

            await _driverRepository.UpdateDriverProfileAsync(driver);
            await _driverRepository.SaveChangesAsync();

            _logger.LogInformation(
                $"Driver {driverProfileId} availability changed to {isAvailable}");

            await _auditLogRepository.CreateAsync(new AuditLog
            {
                UserId = driver.UserId,
                UserRole = UserRole.Driver,
                Action = "Driver Availability Updated",
                Status = AuditStatus.Success,
                EntityType = "DriverProfile",
                EntityId = driver.Id,
                Description =
                    $"Driver availability changed to {isAvailable}",
                CreatedAt = DateTime.UtcNow
            });

            await _auditLogRepository.SaveChangesAsync();

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Driver availability updated successfully",
                Data = new
                {
                    driver.Id,
                    driver.IsAvailable
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while updating availability for driver {driverProfileId}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Error occurred while updating driver availability"
            };
        }
        
        
    }
    public async Task<int?> GetDriverProfileIdByUserIdAsync(int userId)
    {
        var driver = await _driverRepository.GetDriverProfileByUserIdAsync(userId);

        return driver?.Id;
    }

    public async Task<ApiResponse> AddVehicleAsync(int driverProfileId, CreateVehicleRequestDto request)
    {
        try
        {
            var driver = await _driverRepository
                .GetDriverProfileByIdAsync(driverProfileId);

            if (driver == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "Driver profile not found"
                };
            }

            if (driver.Status != ApprovalStatus.Approved)
            {
                return new ApiResponse
                {
                    ResponseCode = "403",
                    ResponseMessage = "Driver is not approved"
                };
            }

            var vehicle = new Vehicle
            {
                DriverProfileId = driverProfileId,
                Make = request.Make!,
                Model = request.Model!,
                Color = request.Color!,
                Year = request.Year,
                LicensePlate = request.LicensePlate
            };

            await _driverRepository.AddVehicleAsync(vehicle);
            await _driverRepository.SaveChangesAsync();

            _logger.LogInformation(
                $"Vehicle added for driver profile {driverProfileId}");

            await _auditLogRepository.CreateAsync(new AuditLog
            {
                UserId = driver.UserId,
                UserRole = UserRole.Driver,
                Action = "Vehicle Added",
                Status = AuditStatus.Success,
                EntityType = "Vehicle",
                EntityId = vehicle.Id,
                Description = "Driver added vehicle details",
                CreatedAt = DateTime.UtcNow
            });

            await _auditLogRepository.SaveChangesAsync();

            return new ApiResponse
            {
                ResponseCode = "201",
                ResponseMessage = "Vehicle added successfully",
                Data = new
                {
                    vehicle.Id,
                    vehicle.Make,
                    vehicle.Model,
                    vehicle.Color,
                    vehicle.Year,
                    vehicle.LicensePlate
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while adding vehicle for driver {driverProfileId}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Error occurred while adding vehicle"
            };
        }
    }
}