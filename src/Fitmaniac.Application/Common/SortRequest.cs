namespace Fitmaniac.Application.Common;

public sealed record SortRequest(string? SortBy = null, bool Descending = false);
