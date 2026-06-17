namespace PeopleDirectory.Domain.Entities;

/// <summary>
/// A durable record of a side-effect (e.g. an email notification) that must be
/// delivered at least once. Rows are written in the same transaction as the
/// business change, then picked up and dispatched by a background processor.
/// This is the transactional Outbox pattern: it guarantees the side-effect is
/// never lost even if the process crashes after the business data is committed.
/// </summary>
public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Logical message type, used to route the payload to a handler.</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Serialized payload (JSON) describing what to dispatch.</summary>
    public string Payload { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>When the message was successfully dispatched; null while pending.</summary>
    public DateTime? ProcessedAt { get; set; }

    /// <summary>Number of dispatch attempts made so far.</summary>
    public int AttemptCount { get; set; }

    /// <summary>The last error recorded for a failed dispatch attempt, if any.</summary>
    public string? LastError { get; set; }

    public static OutboxMessage Create(string type, string payload) => new()
    {
        Type = type,
        Payload = payload,
        CreatedAt = DateTime.UtcNow
    };

    public void MarkProcessed()
    {
        ProcessedAt = DateTime.UtcNow;
        LastError = null;
    }

    public void MarkFailed(string error)
    {
        AttemptCount++;
        LastError = error;
    }
}
