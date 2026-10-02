using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ride_Hailing_API.DTO.Request;
using Ride_Hailing_API.Services.Interfaces;

namespace Ride_Hailing_API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Passenger")]
public class PassengerController : ControllerBase
{
    private readonly IRideService _rideService;
    private readonly IUserService _userService;

    public PassengerController(
        IRideService rideService,
        IUserService userService)
    {
        _rideService = rideService;
        _userService = userService;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetUserId();

        return Ok(
            await _userService.GetProfileAsync(userId));
    }

    [HttpPost("rides")]
    public async Task<IActionResult> RequestRide(
        CreateRideRequestDto request)
    {
        var userId = GetUserId();

        return Ok(
            await _rideService.RequestRideAsync(
                userId,
                request));
    }

    [HttpGet("rides")]
    public async Task<IActionResult> GetMyRides()
    {
        var userId = GetUserId();

        return Ok(
            await _rideService.GetMyRidesAsync(userId));
    }

    [HttpGet("rides/current")]
    public async Task<IActionResult> GetMyCurrentRide()
    {
        var userId = GetUserId();

        return Ok(
            await _rideService.GetMyCurrentRideAsync(userId));
    }

    [HttpPut("rides/{rideId:int}/cancel")]
    public async Task<IActionResult> CancelRide(
        int rideId,
        CancelRideRequestDto request)
    {
        var userId = GetUserId();

        return Ok(
            await _rideService.CancelRideAsync(
                userId,
                rideId,
                request));
    }

    [HttpPut("deactivate")]
    public async Task<IActionResult> DeactivateAccount(
        DeactivateUserRequestDto request)
    {
        var userId = GetUserId();

        return Ok(
            await _userService.DeactivateUserAsync(
                userId,
                request));
    }

    private int GetUserId()
    {
        return int.Parse(
            User.FindFirst(
                ClaimTypes.NameIdentifier)!.Value);
    }
}