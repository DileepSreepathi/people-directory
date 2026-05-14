namespace PeopleDirectory.Application.Interfaces;

public interface IEmailService
{
    Task SendChangeNotificationAsync(string action, string personName, Dictionary<string, (string? OldValue, string? NewValue)> changes);
}
