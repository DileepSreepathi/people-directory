using System.Text;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using PeopleDirectory.Application.Interfaces;

namespace PeopleDirectory.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendChangeNotificationAsync(
        string action, string personName, Dictionary<string, (string? OldValue, string? NewValue)> changes)
    {
        try
        {
            var smtpHost = _config["Email:SmtpHost"] ?? "localhost";
            var smtpPort = int.Parse(_config["Email:SmtpPort"] ?? "587");
            var smtpUser = _config["Email:SmtpUser"] ?? "";
            var smtpPass = _config["Email:SmtpPass"] ?? "";
            var fromEmail = _config["Email:FromEmail"] ?? "noreply@peopledirectory.com";
            var toEmail = _config["Email:ToEmail"] ?? "mark@bluegrassdigital.com";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("People Directory", fromEmail));
            message.To.Add(new MailboxAddress("Admin", toEmail));
            message.Subject = $"People Directory - Record {action}: {personName}";

            var body = new StringBuilder();
            body.AppendLine("<html><body>");
            body.AppendLine($"<h2>Record {action}</h2>");
            body.AppendLine($"<p><strong>Person:</strong> {personName}</p>");
            body.AppendLine($"<p><strong>Action:</strong> {action}</p>");
            body.AppendLine($"<p><strong>Timestamp:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>");

            if (changes.Count > 0)
            {
                body.AppendLine("<h3>Changes:</h3>");
                body.AppendLine("<table border='1' cellpadding='8' cellspacing='0' style='border-collapse: collapse;'>");
                body.AppendLine("<tr><th>Field</th><th>Old Value</th><th>New Value</th></tr>");
                foreach (var (field, (oldVal, newVal)) in changes)
                {
                    body.AppendLine($"<tr><td>{field}</td><td>{oldVal ?? "(empty)"}</td><td>{newVal ?? "(empty)"}</td></tr>");
                }
                body.AppendLine("</table>");
            }

            body.AppendLine("</body></html>");

            message.Body = new TextPart("html") { Text = body.ToString() };

            using var client = new SmtpClient();
            // Auto picks the correct option: None for MailHog (1025), StartTls for 587, SslOnConnect for 465.
            await client.ConnectAsync(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.Auto);
            if (!string.IsNullOrEmpty(smtpUser))
                await client.AuthenticateAsync(smtpUser, smtpPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent for {Action} on {PersonName}", action, personName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email notification for {Action} on {PersonName}", action, personName);
        }
    }
}
