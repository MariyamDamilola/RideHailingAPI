using Ride_Hailing_API.Domain.Entities;

namespace Ride_Hailing_API.Repository.Interfaces;

public interface IAuditLogRepository
{
    Task<AuditLog> CreateAsync(AuditLog auditLog);
    Task SaveChangesAsync();
}