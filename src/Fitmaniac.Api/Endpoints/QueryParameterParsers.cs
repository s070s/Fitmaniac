using System.Globalization;

namespace Fitmaniac.Api.Endpoints;

internal static class QueryParameterParsers
{
    public static bool TryParseOptionalDateTime(string? value, string parameterName, out DateTime? result, out Dictionary<string, string[]>? errors)
    {
        result = null;
        errors = null;

        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed))
        {
            result = parsed;
            return true;
        }

        errors = new Dictionary<string, string[]>
        {
            [parameterName] = [$"The value '{value}' is not a valid date/time."]
        };
        return false;
    }
}