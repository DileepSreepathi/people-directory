using Microsoft.EntityFrameworkCore;
using PeopleDirectory.Domain.Entities;
using PeopleDirectory.Domain.Interfaces;
using PeopleDirectory.Infrastructure.Data;

namespace PeopleDirectory.Infrastructure.Repositories;

public class OutboxRepository : IOutboxRepository
{
    private readonly AppDbContext _context;

    public OutboxRepository(AppDbContext context) => _context = context;

    public async Task AddAsync(OutboxMessage message) =>
        await _context.OutboxMessages.AddAsync(message);

    public async Task<IReadOnlyList<OutboxMessage>> GetUnprocessedAsync(int batchSize, int maxAttempts) =>
        await _context.OutboxMessages
            .Where(m => m.ProcessedAt == null && m.AttemptCount < maxAttempts)
            .OrderBy(m => m.CreatedAt)
            .Take(batchSize)
            .ToListAsync();

    public void Update(OutboxMessage message) => _context.OutboxMessages.Update(message);

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}
