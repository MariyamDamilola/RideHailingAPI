
using Ride_Hailing_API.Data;
using Ride_Hailing_API.Domain.Entities;
using Ride_Hailing_API.Repository.Interfaces;

namespace Ride_Hailing_API.Repository.Implementations;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext _context;

    public AuditLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AuditLog> CreateAsync(AuditLog auditLog)
    {
        await _context.AuditLogs.AddAsync(auditLog);
        return auditLog;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}