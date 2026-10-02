using Ride_Hailing_API.Domain.Enum;

namespace Ride_Hailing_API.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string PasswordHash { get; set; }
    public UserRole Role { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsPhoneNumberVerified { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    public DriverProfile? DriverProfile { get; set; }
    public ICollection<Ride> RidesAsPassenger { get; set; } = new List<Ride>();
    public ICollection<EmailOtp> EmailOtps { get; set; } = new List<EmailOtp>();
    public ICollection<PhoneOtp> PhoneOtps { get; set; } = new List<PhoneOtp>();
    public ICollection<PasswordResetOtp> PasswordResetOtps { get; set; } = new List<PasswordResetOtp>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    
    public ICollection<RideStatusHistory> RideStatusHistories { get; set; } = new List<RideStatusHistory>();
}