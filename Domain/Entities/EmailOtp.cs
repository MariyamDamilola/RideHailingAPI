namespace Ride_Hailing_API.Domain.Entities;

public class EmailOtp
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Code { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;
    public User User { get; set; }
}