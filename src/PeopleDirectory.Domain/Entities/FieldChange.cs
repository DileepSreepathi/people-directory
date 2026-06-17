namespace PeopleDirectory.Domain.Entities;

/// <summary>
/// A value object describing a single field that changed on an entity,
/// capturing the previous and new values. Used for audit logging and
/// change notifications.
/// </summary>
public sealed record FieldChange(string Field, string? OldValue, string? NewValue);
