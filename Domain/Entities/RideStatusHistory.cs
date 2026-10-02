using Ride_Hailing_API.Domain.Enum;

namespace Ride_Hailing_API.Domain.Entities;

public class RideStatusHistory
{
    public int Id { get; set; }
    public int RideId { get; set; }
    public RideStatus Status { get; set; }
    public int ChangedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Ride Ride { get; set; }
    public User ChangedByUser { get; set; }
}