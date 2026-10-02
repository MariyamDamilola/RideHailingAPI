using Microsoft.EntityFrameworkCore;
using Ride_Hailing_API.Domain.Entities;

namespace Ride_Hailing_API.Data;

public class AppDbContext : DbContext
{
      public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<DriverProfile> DriverProfiles { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Ride> Rides { get; set; }
    public DbSet<RideStatusHistory> RideStatusHistories { get; set; }
    public DbSet<EmailOtp> EmailOtps { get; set; }
    public DbSet<PhoneOtp> PhoneOtps { get; set; }
    public DbSet<PasswordResetOtp> PasswordResetOtps { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.PhoneNumber).IsUnique();

            entity.Property(u => u.Role)
                  .HasConversion<string>();
        });

        // Driver Profile
        modelBuilder.Entity<DriverProfile>(entity =>
        {
            entity.HasOne(d => d.User)
                  .WithOne(u => u.DriverProfile)
                  .HasForeignKey<DriverProfile>(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(d => d.UserId).IsUnique();

            entity.Property(d => d.Status)
                  .HasConversion<string>();
        });

        // Vehicle
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasOne(v => v.DriverProfile)
                  .WithMany(d => d.Vehicles)
                  .HasForeignKey(v => v.DriverProfileId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(v => v.LicensePlate).IsUnique();
        });

        // Ride
        modelBuilder.Entity<Ride>(entity =>
        {
            entity.HasIndex(r => r.RideReference).IsUnique();

            entity.Property(r => r.Status)
                  .HasConversion<string>();

            entity.HasOne(r => r.Passenger)
                  .WithMany(u => u.RidesAsPassenger)
                  .HasForeignKey(r => r.PassengerId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.DriverProfile)
                  .WithMany(d => d.Rides)
                  .HasForeignKey(r => r.DriverProfileId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Ride Status History
        modelBuilder.Entity<RideStatusHistory>(entity =>
        {
            entity.Property(h => h.Status)
                  .HasConversion<string>();

            entity.HasOne(h => h.Ride)
                  .WithMany(r => r.StatusHistories)
                  .HasForeignKey(h => h.RideId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(h => h.ChangedByUser)
                  .WithMany()
                  .HasForeignKey(h => h.ChangedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Email OTP
        modelBuilder.Entity<EmailOtp>(entity =>
        {
            entity.HasOne(e => e.User)
                  .WithMany(u => u.EmailOtps)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Phone OTP
        modelBuilder.Entity<PhoneOtp>(entity =>
        {
            entity.HasOne(p => p.User)
                  .WithMany(u => u.PhoneOtps)
                  .HasForeignKey(p => p.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Password Reset OTP
        modelBuilder.Entity<PasswordResetOtp>(entity =>
        {
            entity.HasOne(p => p.User)
                  .WithMany(u => u.PasswordResetOtps)
                  .HasForeignKey(p => p.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Notification
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.Property(n => n.Type)
                  .HasConversion<string>();

            entity.Property(n => n.Status)
                  .HasConversion<string>();

            entity.HasOne(n => n.User)
                  .WithMany(u => u.Notifications)
                  .HasForeignKey(n => n.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(n => n.Ride)
                  .WithMany()
                  .HasForeignKey(n => n.RideId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Audit Log
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.Property(a => a.UserRole)
                  .HasConversion<string>();

            entity.Property(a => a.Status)
                  .HasConversion<string>();

            entity.HasOne(a => a.User)
                  .WithMany(u => u.AuditLogs)
                  .HasForeignKey(a => a.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}