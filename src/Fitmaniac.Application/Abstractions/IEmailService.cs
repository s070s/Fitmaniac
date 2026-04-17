namespace Fitmaniac.Application.Abstractions;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken ct = default);
    Task SendEmailConfirmationAsync(string to, string token, CancellationToken ct = default);
    Task SendPasswordResetAsync(string to, string token, CancellationToken ct = default);
}
