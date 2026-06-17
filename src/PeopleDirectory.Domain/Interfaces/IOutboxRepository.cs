using PeopleDirectory.Domain.Entities;

namespace PeopleDirectory.Domain.Interfaces;

public interface IOutboxRepository
{
    /// <summary>Stages a message to be saved as part of the current unit of work.</summary>
    Task AddAsync(OutboxMessage message);

    /// <summary>Returns pending (unprocessed) messages, oldest first, up to <paramref name="batchSize"/>.</summary>
    Task<IReadOnlyList<OutboxMessage>> GetUnprocessedAsync(int batchSize, int maxAttempts);

    void Update(OutboxMessage message);

    Task<int> SaveChangesAsync();
}
