using Fitmaniac.Application.Common;

namespace Fitmaniac.Api.Endpoints;

public static class ServiceResultExtensions
{
    public static IResult ToResult<T>(this ServiceResult<T> result)
    {
        return result.StatusCode switch
        {
            200 => Results.Ok(result.Value),
            201 => Results.Created(string.Empty, result.Value),
            204 => Results.NoContent(),
            400 => Results.BadRequest(result.ErrorMessage),
            401 => Results.Unauthorized(),
            403 => Results.Forbid(),
            404 => Results.NotFound(result.ErrorMessage),
            409 => Results.Conflict(result.ErrorMessage),
            _ => Results.Problem(result.ErrorMessage)
        };
    }
}
