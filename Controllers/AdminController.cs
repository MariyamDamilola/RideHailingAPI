using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ride_Hailing_API.DTO.Request;
using Ride_Hailing_API.Services.Interfaces;

namespace Ride_Hailing_API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        return Ok(await _adminService.GetUsersAsync());
    }

    [HttpGet("users/{userId:int}")]
    public async Task<IActionResult> GetUserById(int userId)
    {
        return Ok(await _adminService.GetUserByIdAsync(userId));
    }

    [HttpGet("drivers/pending")]
    public async Task<IActionResult> GetPendingDrivers()
    {
        return Ok(await _adminService.GetPendingDriversAsync());
    }

    [HttpPut("drivers/{driverProfileId:int}/approve")]
    public async Task<IActionResult> ApproveDriver(int driverProfileId)
    {
        return Ok(await _adminService.ApproveDriverAsync(driverProfileId));
    }

    [HttpPut("drivers/{driverProfileId:int}/reject")]
    public async Task<IActionResult> RejectDriver(
        int driverProfileId,
        RejectDriverRequestDto request)
    {
        return Ok(await _adminService.RejectDriverAsync(
            driverProfileId,
            request));
    }

    [HttpGet("rides")]
    public async Task<IActionResult> GetRides()
    {
        return Ok(await _adminService.GetRidesAsync());
    }

    [HttpGet("audit-logs")]
    public async Task<IActionResult> GetAuditLogs()
    {
        return Ok(await _adminService.GetAuditLogsAsync());
    }
}