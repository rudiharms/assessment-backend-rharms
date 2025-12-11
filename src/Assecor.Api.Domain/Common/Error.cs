namespace Assecor.Api.Domain.Common;

public record Error(string Code, string Message);

public static class Errors
{
    public static readonly Error InvalidAddressZipCode = new(Codes.InvalidAddressZipCode, "The address zip code is invalid.");
    public static readonly Error InvalidAddressCity = new(Codes.InvalidAddressCity, "The address city is invalid.");
    public static readonly Error InvalidColor = new(Codes.InvalidColor, "The color is invalid.");

    public static Error FileNotFound(string filePath)
    {
        return new Error(Codes.FileNotFound, $"CSV file not found at path: {filePath}");
    }

    public static Error CsvLoadingFailed(string message)
    {
        return new Error(Codes.CsvLoadingFailed, $"CSV file loading failed with message: {message}");
    }

    public static Error CsvParsingFailed(string message)
    {
        return new Error(Codes.CsvParsingFailed, $"CSV file parsing failed with message: {message}");
    }
}

public static class Codes
{
    public const string InvalidAddressZipCode = nameof(InvalidAddressZipCode);
    public const string InvalidAddressCity = nameof(InvalidAddressCity);
    public const string InvalidColor = nameof(InvalidColor);
    public const string FileNotFound = nameof(FileNotFound);
    public const string CsvLoadingFailed = nameof(CsvLoadingFailed);
    public const string CsvParsingFailed = nameof(CsvParsingFailed);
}
