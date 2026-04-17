using Fitmaniac.Application.Abstractions;
using Fitmaniac.Shared.Constants;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Fitmaniac.Infrastructure.Identity;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // Design-time only constructor (no DI)
    internal CurrentUserService() : this(new HttpContextAccessor()) { }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public int? UserId
    {
        get
        {
            var claim = User?.FindFirstValue(ClaimTypesExtended.UserId)
                        ?? User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out var id) ? id : null;
        }
    }

    public string? UserName => User?.Identity?.Name;

    public IReadOnlyList<string> Roles =>
        User?.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList() ?? [];

    public bool IsInRole(string role) => User?.IsInRole(role) ?? false;
}
