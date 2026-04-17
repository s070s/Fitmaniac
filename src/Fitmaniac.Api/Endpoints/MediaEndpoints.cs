using Fitmaniac.Application.Abstractions;

namespace Fitmaniac.Api.Endpoints;

public static class MediaEndpoints
{
    public static IEndpointRouteBuilder MapMediaEndpoints(this IEndpointRouteBuilder e)
    {
        var g = e.MapGroup("/media").RequireAuthorization();

        g.MapPost("/upload", async (IFormFile file, IFileStorage storage, IValidationService validation, CancellationToken ct) =>
        {
            await using var validationStream = file.OpenReadStream();
            if (!validation.IsValidImage(validationStream, file.FileName))
                return Results.BadRequest("Invalid image type or size.");

            await using var stream = file.OpenReadStream();
            var path = await storage.SaveAsync(stream, file.FileName, "uploads", ct);
            return Results.Ok(new { url = path });
        });

        return e;
    }
}
