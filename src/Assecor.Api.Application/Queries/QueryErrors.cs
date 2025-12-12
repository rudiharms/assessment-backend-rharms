using Assecor.Api.Domain.Enums;

namespace Assecor.Api.Domain.Common;

public static class QueryErrors
{
    public static Error PersonNotFound(int id)
    {
        return new Error(Codes.PersonNotFoundCode, $"Person could not be found for Id: {id}");
    }

    public static Error PersonsQueryFailed()
    {
        return new Error(Codes.PersonsQueryFailedCode, "Query to retrieve persons failed");
    }

    public static Error InvalidColor(string colorName)
    {
        return new Error(
            Codes.InvalidColorCode,
            $"Invalid color: {colorName}. Valid values are: {string.Join(", ", Enum.GetNames<ColorName>())}"
        );
    }

    public static Error ColorQueryFailed(ColorName colorName)
    {
        return new Error(Codes.ColorQueryFailedCode, $"Query to retrieve persons failed for color: {colorName}");
    }

    public static Error UnknownError(string message)
    {
        return new Error(Codes.UnknownErrorCode, $"Unknown error occured with message: {message}");
    }

    public static class Codes
    {
        public const string PersonNotFoundCode = nameof(PersonNotFoundCode);
        public const string PersonsQueryFailedCode = nameof(PersonsQueryFailedCode);
        public const string ColorQueryFailedCode = nameof(ColorQueryFailedCode);
        public const string UnknownErrorCode = nameof(UnknownErrorCode);
        public const string InvalidColorCode = nameof(InvalidColorCode);
    }
}
