using Fitmaniac.Application.Abstractions;
using Fitmaniac.Application.Services;
using Fitmaniac.Shared.DTOs.Users;
using Fitmaniac.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Fitmaniac.Api.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserProfileEndpoints(this IEndpointRouteBuilder e)
    {
        var g = e.MapGroup("/users").RequireAuthorization();

        g.MapGet("/me", async (ICurrentUserService cur, IUserService svc, CancellationToken ct) =>
            (await svc.GetCurrentUserAsync(cur.UserId!.Value, ct)).ToResult());

        g.MapPut("/me", async (UpdateUserDto dto, ICurrentUserService cur, IUserService svc, CancellationToken ct) =>
            (await svc.UpdateProfileAsync(cur.UserId!.Value, dto, ct)).ToResult());

        g.MapPost("/me/photo", async (IFormFile photo, ICurrentUserService cur, IUserService svc, CancellationToken ct) =>
            (await svc.UploadPhotoAsync(cur.UserId!.Value, photo, ct)).ToResult());

        return e;
    }

    public static IEndpointRouteBuilder MapAdminUserEndpoints(this IEndpointRouteBuilder e)
    {
        var g = e.MapGroup("/admin/users").RequireAuthorization(PolicyNames.Admin);

        g.MapGet("/", async ([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string? sortBy,
            [FromQuery] bool descending, [FromQuery] string? search, IAdminUserService svc, CancellationToken ct) =>
            (await svc.GetUsersAsync(page == 0 ? 1 : page, pageSize == 0 ? 20 : pageSize, sortBy, descending, search, ct)).ToResult());

        g.MapGet("/statistics", async (IAdminUserService svc, CancellationToken ct) =>
            (await svc.GetStatisticsAsync(ct)).ToResult());

        g.MapGet("/{id:int}", async (int id, IAdminUserService svc, CancellationToken ct) =>
            (await svc.GetByIdAsync(id, ct)).ToResult());

        g.MapPost("/", async (CreateUserDto dto, IAdminUserService svc, CancellationToken ct) =>
            (await svc.CreateAsync(dto, ct)).ToResult());

        g.MapPut("/", async (UpdateUserDto dto, IAdminUserService svc, CancellationToken ct) =>
            (await svc.UpdateAsync(dto, ct)).ToResult());

        g.MapDelete("/{id:int}", async (int id, IAdminUserService svc, CancellationToken ct) =>
            (await svc.DeleteAsync(id, ct)).ToResult());

        g.MapPost("/{id:int}/enable", async (int id, IAdminUserService svc, CancellationToken ct) =>
            (await svc.EnableAsync(id, ct)).ToResult());

        g.MapPost("/{id:int}/disable", async (int id, IAdminUserService svc, CancellationToken ct) =>
            (await svc.DisableAsync(id, ct)).ToResult());

        return e;
    }
}
