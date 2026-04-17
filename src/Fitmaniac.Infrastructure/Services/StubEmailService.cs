using Fitmaniac.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace Fitmaniac.Infrastructure.Services;

/// <summary>Stub email service — logs to console in development; replace with real provider in production.</summary>
public sealed class StubEmailService : IEmailService
{
    private readonly ILogger<StubEmailService> _logger;

    public StubEmailService(ILogger<StubEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
    {
        _logger.LogInformation("[StubEmail] To={To} Subject={Subject}", to, subject);
        return Task.CompletedTask;
    }

    public Task SendEmailConfirmationAsync(string to, string confirmLink, CancellationToken ct = default)
    {
        _logger.LogInformation("[StubEmail] Confirmation link for {To}: {Link}", to, confirmLink);
        return Task.CompletedTask;
    }

    public Task SendPasswordResetAsync(string to, string resetLink, CancellationToken ct = default)
    {
        _logger.LogInformation("[StubEmail] Password reset link for {To}: {Link}", to, resetLink);
        return Task.CompletedTask;
    }
}
