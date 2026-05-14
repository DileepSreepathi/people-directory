namespace PeopleDirectory.Domain.Entities;

public class AuditLog
{
    public int Id { get; set; }
    public int PersonId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? ChangesJson { get; set; }
    public string PerformedBy { get; set; } = string.Empty;
    public DateTime PerformedAt { get; set; } = DateTime.UtcNow;

    public Person Person { get; set; } = null!;
}
