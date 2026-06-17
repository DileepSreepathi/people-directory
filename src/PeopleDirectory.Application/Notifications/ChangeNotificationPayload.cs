using PeopleDirectory.Domain.Entities;

namespace PeopleDirectory.Application.Notifications;

/// <summary>
/// Serializable payload stored in the outbox for a person change notification.
/// The background outbox processor deserializes this and hands it to the
/// <see cref="Interfaces.IEmailService"/>.
/// </summary>
public sealed record ChangeNotificationPayload(
    string Action,
    string PersonName,
    List<FieldChange> Changes)
{
    /// <summary>The outbox message <c>Type</c> discriminator for this payload.</summary>
    public const string MessageType = "PersonChangeNotification";
}
