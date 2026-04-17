namespace Fitmaniac.Application.Common;

public sealed record ServiceResult<T>(T? Value, int StatusCode, string? ErrorMessage)
{
    public bool IsSuccess => StatusCode is >= 200 and < 300;

    public static ServiceResult<T> Ok(T value) => new(value, 200, null);
    public static ServiceResult<T> Created(T value) => new(value, 201, null);
    public static ServiceResult<T> NoContent() => new(default, 204, null);
    public static ServiceResult<T> BadRequest(string message) => new(default, 400, message);
    public static ServiceResult<T> Unauthorized(string message = "Unauthorized") => new(default, 401, message);
    public static ServiceResult<T> Forbidden(string message = "Forbidden") => new(default, 403, message);
    public static ServiceResult<T> NotFound(string message = "Not found") => new(default, 404, message);
    public static ServiceResult<T> Conflict(string message) => new(default, 409, message);
    public static ServiceResult<T> Error(string message) => new(default, 500, message);
}
