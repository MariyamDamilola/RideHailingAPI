using Ride_Hailing_API.Domain.Enum;

namespace Ride_Hailing_API.Domain.Entities;

public class AuditLog
{
     public int Id { get; set; }
    public int UserId { get; set; }
    public UserRole UserRole { get; set; }
    public string? Action { get; set; }
    public AuditStatus Status { get; set; }
    public string EntityType { get; set; }
    public int? EntityId { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public User User { get; set; }
}