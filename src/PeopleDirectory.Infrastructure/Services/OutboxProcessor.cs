using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PeopleDirectory.Application.Interfaces;
using PeopleDirectory.Application.Notifications;
using PeopleDirectory.Domain.Interfaces;

namespace PeopleDirectory.Infrastructure.Services;

/// <summary>
/// Background worker that drains the transactional outbox. It periodically reads
/// pending messages and dispatches them (currently email notifications) with
/// retry and backoff via the per-message <c>AttemptCount</c>. This replaces the
/// previous fire-and-forget <c>Task.Run</c> email send and gives at-least-once,
/// crash-durable delivery: messages survive process restarts because they live
/// in the database until successfully dispatched.
/// </summary>
public class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessor> _logger;

    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(10);
    private const int BatchSize = 20;
    private const int MaxAttempts = 5;

    public OutboxProcessor(IServiceScopeFactory scopeFactory, ILogger<OutboxProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox processor started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                // Never let a transient failure kill the worker loop.
                _logger.LogError(ex, "Unexpected error while processing the outbox.");
            }

            try
            {
                await Task.Delay(PollInterval, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("Outbox processor stopping.");
    }

    private async Task ProcessPendingAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var outbox = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        var messages = await outbox.GetUnprocessedAsync(BatchSize, MaxAttempts);
        if (messages.Count == 0)
            return;

        foreach (var message in messages)
        {
            if (stoppingToken.IsCancellationRequested)
                break;

            try
            {
                await DispatchAsync(message, emailService);
                message.MarkProcessed();
            }
            catch (Exception ex)
            {
                message.MarkFailed(ex.Message);
                _logger.LogWarning(ex,
                    "Outbox message {MessageId} failed (attempt {Attempt}/{Max}).",
                    message.Id, message.AttemptCount, MaxAttempts);
            }

            outbox.Update(message);
        }

        await outbox.SaveChangesAsync();
    }

    private static async Task DispatchAsync(Domain.Entities.OutboxMessage message, IEmailService emailService)
    {
        switch (message.Type)
        {
            case ChangeNotificationPayload.MessageType:
                var payload = JsonSerializer.Deserialize<ChangeNotificationPayload>(message.Payload)
                    ?? throw new InvalidOperationException("Outbox payload could not be deserialized.");

                var changes = payload.Changes.ToDictionary(
                    c => c.Field,
                    c => (c.OldValue, c.NewValue));

                await emailService.SendChangeNotificationAsync(payload.Action, payload.PersonName, changes);
                break;

            default:
                throw new NotSupportedException($"Unknown outbox message type '{message.Type}'.");
        }
    }
}
