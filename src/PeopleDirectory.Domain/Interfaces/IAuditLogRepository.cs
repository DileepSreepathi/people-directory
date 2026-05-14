using PeopleDirectory.Domain.Entities;

namespace PeopleDirectory.Domain.Interfaces;

public interface IAuditLogRepository : IRepository<AuditLog>
{
    Task<IEnumerable<AuditLog>> GetByPersonIdAsync(int personId);
}
