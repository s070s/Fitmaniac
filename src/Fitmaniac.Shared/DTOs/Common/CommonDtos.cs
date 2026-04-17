namespace Fitmaniac.Shared.DTOs.Common;

public sealed record PagedResultDto<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}

public sealed record ErrorResponseDto(string Message, IReadOnlyList<string>? Errors = null);

public sealed record ProblemDetailsDto(
    string? Type,
    string? Title,
    int? Status,
    string? Detail,
    string? Instance);
