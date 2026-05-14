using Microsoft.EntityFrameworkCore;
using PeopleDirectory.Domain.Entities;
using PeopleDirectory.Domain.Interfaces;
using PeopleDirectory.Infrastructure.Data;

namespace PeopleDirectory.Infrastructure.Repositories;

public class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<AuditLog>> GetByPersonIdAsync(int personId)
    {
        return await DbSet
            .Where(a => a.PersonId == personId)
            .OrderByDescending(a => a.PerformedAt)
            .ToListAsync();
    }
}
