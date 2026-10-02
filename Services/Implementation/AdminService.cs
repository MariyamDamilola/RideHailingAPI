using Ride_Hailing_API.Domain.Entities;
using Ride_Hailing_API.Domain.Enum;
using Ride_Hailing_API.DTO.Request;
using Ride_Hailing_API.DTO.Response;
using Ride_Hailing_API.Repository.Interfaces;
using Ride_Hailing_API.Services.Interfaces;
using RideHailingAPI.Helpers;

namespace Ride_Hailing_API.Services.Implementation;

public class AdminService : IAdminService
{
     private readonly IAdminRepository _adminRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        IAdminRepository adminRepository,
        IAuditLogRepository auditLogRepository,
        IEmailService emailService,
        ISmsService smsService,
        ILogger<AdminService> logger)
    {
        _adminRepository = adminRepository;
        _auditLogRepository = auditLogRepository;
        _emailService = emailService;
        _smsService = smsService;
        _logger = logger;
    }

    public async Task<ApiResponse> GetUsersAsync()
    {
        try
        {
            var users = await _adminRepository.GetUsersAsync();

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Users retrieved successfully",
                Data = users.Select(user => new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.PhoneNumber,
                    user.Role,
                    user.IsEmailVerified,
                    user.IsPhoneNumberVerified,
                    user.IsActive,
                    user.CreatedAt
                })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving users");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Error occurred while retrieving users"
            };
        }
    }

    public async Task<ApiResponse> GetUserByIdAsync(int userId)
    {
        try
        {
            var user = await _adminRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "User not found"
                };
            }

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "User retrieved successfully",
                Data = new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.PhoneNumber,
                    user.Role,
                    user.IsEmailVerified,
                    user.IsPhoneNumberVerified,
                    user.IsActive,
                    user.CreatedAt,
                    user.UpdatedAt
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while retrieving user {userId}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Error occurred while retrieving user"
            };
        }
    }

    public async Task<ApiResponse> GetPendingDriversAsync()
    {
        try
        {
            var drivers =
                await _adminRepository.GetPendingDriversAsync();

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Pending drivers retrieved successfully",
                Data = drivers.Select(driver => new
                {
                    driver.User.Id,
                    driver.Status,
                    driver.ApprovalReason,
                    driver.User.FullName,
                    driver.User.Email,
                    driver.User.PhoneNumber,
                    Vehicles = driver.Vehicles.Select(vehicle => new
                    {
                        vehicle.Id,
                        vehicle.Make,
                        vehicle.Model,
                        vehicle.Color,
                        vehicle.Year,
                        vehicle.LicensePlate
                    })
                })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while retrieving pending drivers");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage =
                    "Error occurred while retrieving pending drivers"
            };
        }
    }

    public async Task<ApiResponse> ApproveDriverAsync(
        int driverProfileId)
    {
        try
        {
            var driver =
                await _adminRepository.GetDriverProfileByIdAsync(
                    driverProfileId);

            if (driver == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "Driver profile not found"
                };
            }

            if (driver.Status == ApprovalStatus.Approved)
            {
                return new ApiResponse
                {
                    ResponseCode = "400",
                    ResponseMessage = "Driver is already approved"
                };
            }

            driver.Status = ApprovalStatus.Approved;
            driver.ApprovalReason = null;
            driver.ApprovedAt = DateTime.UtcNow;

            await _adminRepository.UpdateDriverProfileAsync(driver);
            await _adminRepository.SaveChangesAsync();

            _logger.LogInformation(
                $"Driver {driverProfileId} approved successfully");

            await _emailService.SendDriverApprovedEmailAsync(
                driver.User.Email,
                driver.User.FullName,
                driver.Id);

            await _smsService.SendSmsAsync(
                driver.User.PhoneNumber,
                SmsUtils.GetDriverApprovedSms(
                    driver.User.FullName,
                    driver.Id));

            await _auditLogRepository.CreateAsync(new AuditLog
            {
                UserId = driver.UserId,
                UserRole = UserRole.Driver,
                Action = "Driver Approved",
                Status = AuditStatus.Success,
                EntityType = "DriverProfile",
                EntityId = driver.Id,
                Description =
                    $"Driver profile {driver.Id} was approved",
                CreatedAt = DateTime.UtcNow
            });

            await _auditLogRepository.SaveChangesAsync();

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Driver approved successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while approving driver {driverProfileId}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Error occurred while approving driver"
            };
        }
    }

    public async Task<ApiResponse> RejectDriverAsync(
        int driverProfileId,
        RejectDriverRequestDto request)
    {
        try
        {
            var driver =
                await _adminRepository.GetDriverProfileByIdAsync(
                    driverProfileId);

            if (driver == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "Driver profile not found"
                };
            }

            if (driver.Status == ApprovalStatus.Rejected)
            {
                return new ApiResponse
                {
                    ResponseCode = "400",
                    ResponseMessage = "Driver is already rejected"
                };
            }

            driver.Status = ApprovalStatus.Rejected;
            driver.ApprovalReason = request.RejectionReason;
            driver.ApprovedAt = null;

            await _adminRepository.UpdateDriverProfileAsync(driver);
            await _adminRepository.SaveChangesAsync();

            _logger.LogInformation(
                $"Driver {driverProfileId} rejected successfully");

            await _emailService.SendDriverRejectedEmailAsync(
                driver.User.Email,
                driver.User.FullName,
                driver.Id,
                request.RejectionReason!);

            await _smsService.SendSmsAsync(
                driver.User.PhoneNumber,
                SmsUtils.GetDriverRejectedSms(
                    driver.User.FullName,
                    driver.Id,
                    request.RejectionReason!));

            await _auditLogRepository.CreateAsync(new AuditLog
            {
                UserId = driver.UserId,
                UserRole = UserRole.Driver,
                Action = "Driver Rejected",
                Status = AuditStatus.Success,
                EntityType = "DriverProfile",
                EntityId = driver.Id,
                Description =
                    $"Driver profile {driver.Id} was rejected. " +
                    $"Reason: {request.RejectionReason}",
                CreatedAt = DateTime.UtcNow
            });

            await _auditLogRepository.SaveChangesAsync();

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Driver rejected successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while rejecting driver {driverProfileId}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Error occurred while rejecting driver"
            };
        }
    }

    public async Task<ApiResponse> GetRidesAsync()
    {
        try
        {
            var rides = await _adminRepository.GetRidesAsync();

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Rides retrieved successfully",
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
                            ride.Passenger.Email,
                            ride.Passenger.PhoneNumber
                        },

                    Driver = ride.DriverProfile == null
                        ? null
                        : new
                        {
                            ride.DriverProfile.Id,
                            Name = ride.DriverProfile.User.FullName,
                            PhoneNumber =
                                ride.DriverProfile.User.PhoneNumber
                        }
                })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while retrieving rides");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Error occurred while retrieving rides"
            };
        }
    }

    public async Task<ApiResponse> GetAuditLogsAsync()
    {
        try
        {
            var logs = await _adminRepository.GetAuditLogsAsync();

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Audit logs retrieved successfully",
                Data = logs.Select(log => new
                {
                    log.Id,
                    log.UserId,
                    log.UserRole,
                    log.Action,
                    log.Status,
                    log.EntityType,
                    log.EntityId,
                    log.Description,
                    log.CreatedAt
                })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while retrieving audit logs");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Error occurred while retrieving audit logs"
            };
        }
    }
}