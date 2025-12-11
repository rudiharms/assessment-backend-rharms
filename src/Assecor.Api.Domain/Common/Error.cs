namespace Assecor.Api.Domain.Common;

public record Error(string Code, string Message);

public static class Errors
{
    public static readonly Error InvalidAddressZipCode = new(Codes.InvalidAddressZipCode, "The address zip code is invalid.");
    public static readonly Error InvalidAddressCity = new(Codes.InvalidAddressCity, "The address city is invalid.");
    public static readonly Error InvalidColor = new(Codes.InvalidColor, "The color is invalid.");
}

public static class Codes
{
    public const string InvalidAddressZipCode = nameof(InvalidAddressZipCode);
    public const string InvalidAddressCity = nameof(InvalidAddressCity);
    public const string InvalidColor = nameof(InvalidColor);
}
