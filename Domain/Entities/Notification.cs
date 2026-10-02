using Ride_Hailing_API.Domain.Enum;

namespace Ride_Hailing_API.Domain.Entities;

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? RideId { get; set; }
    public NotificationType Type { get; set; }
    public string Recipient { get; set; }
    public string Subject { get; set; }
    public string Message { get; set; }
    public NotificationStatus Status { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
    public User User { get; set; }
    public Ride? Ride { get; set; }
}