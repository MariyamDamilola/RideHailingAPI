using Ride_Hailing_API.Domain.Enum;

namespace Ride_Hailing_API.Domain.Entities;

public class DriverProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public bool IsAvailable { get; set; }
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
    public string? ApprovalReason { get; set; }
    public DateTime? ApprovedAt { get; set; } 
    public User User { get; set; }
    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    public ICollection<Ride> Rides { get; set; } = new List<Ride>();

}