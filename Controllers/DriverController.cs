using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ride_Hailing_API.DTO.Request;
using Ride_Hailing_API.Services.Interfaces;

namespace Ride_Hailing_API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Driver")]
public class DriverController : ControllerBase
{
    private readonly IDriverService _driverService;
    private readonly IRideService _rideService;

    public DriverController(
        IDriverService driverService,
        IRideService rideService)
    {
        _driverService = driverService;
        _rideService = rideService;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var driverProfileId = await GetDriverProfileId();

        return Ok(await _driverService.GetProfileAsync(driverProfileId));
    }
    
    [HttpPost("vehicles")]
    public async Task<IActionResult> AddVehicle(
        CreateVehicleRequestDto request)
    {
        var driverProfileId = await GetDriverProfileId();

        return Ok(
            await _driverService.AddVehicleAsync(
                driverProfileId,
                request));
    }

    [HttpGet("rides")]
    public async Task<IActionResult> GetAssignedRides()
    {
        var driverProfileId = await GetDriverProfileId();

        return Ok(
            await _driverService.GetAssignedRidesAsync(
                driverProfileId));
    }

    [HttpGet("rides/current")]
    public async Task<IActionResult> GetCurrentRide()
    {
        var driverProfileId = await GetDriverProfileId();

        return Ok(
            await _driverService.GetCurrentRideAsync(
                driverProfileId));
    }

    [HttpPut("availability")]
    public async Task<IActionResult> SetAvailability(
        [FromQuery] bool isAvailable)
    {
        var driverProfileId = await GetDriverProfileId();

        return Ok(
            await _driverService.SetAvailabilityAsync(
                driverProfileId,
                isAvailable));
    }

    [HttpPut("rides/{rideId:int}/accept")]
    public async Task<IActionResult> AcceptRide(int rideId)
    {
        var driverProfileId = await GetDriverProfileId();

        return Ok(
            await _rideService.AcceptRideAsync(
                driverProfileId,
                rideId));
    }

    [HttpPut("rides/{rideId:int}/status")]
    public async Task<IActionResult> UpdateRideStatus(
        int rideId,
        UpdateRideStatusRequestDto request)
    {
        var driverProfileId = await GetDriverProfileId();

        return Ok(
            await _rideService.UpdateRideStatusAsync(
                driverProfileId,
                rideId,
                request));
    }

    private async Task<int> GetDriverProfileId()
    {
        var userId = int.Parse(
            User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var driverProfileId = await _driverService
            .GetDriverProfileIdByUserIdAsync(userId);

        if (driverProfileId == null)
        {
            throw new Exception("Driver profile not found");
        }

        return driverProfileId.Value;
    }
}
