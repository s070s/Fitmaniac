namespace Fitmaniac.Application.Abstractions;

public interface ICurrentUserService
{
    int? UserId { get; }
    string? UserName { get; }
    IReadOnlyList<string> Roles { get; }
    bool IsInRole(string role);
}
