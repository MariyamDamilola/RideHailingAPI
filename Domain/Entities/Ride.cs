using Ride_Hailing_API.Domain.Enum;

namespace Ride_Hailing_API.Domain.Entities;

public class Ride
{
    public int Id { get; set; }
    public string? RideReference { get; set; }
    public int PassengerId { get; set; }
    public int? DriverProfileId { get; set; }
    public string? PickupAddress { get; set; }
    public string? DestinationAddress { get; set; }
    public RideStatus Status { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public User? Passenger { get; set; }
    public DriverProfile? DriverProfile { get; set; }
    public ICollection<RideStatusHistory> StatusHistories { get; set; } = new List<RideStatusHistory>();
}