namespace Assecor.Api.Domain.Common;

public static class Errors
{
    public static readonly Error InvalidAddressZipCode = new(Codes.InvalidAddressZipCodeCode, "The address zip code is invalid");
    public static readonly Error InvalidAddressCity = new(Codes.InvalidAddressCityCode, "The address city is invalid");
    public static readonly Error AddressIsMissing = new(Codes.AddressIsMissingCode, "The address  is missing");
    public static readonly Error InvalidColor = new(Codes.InvalidColorCode, "The color is invalid");

    public static Error PersonNotFound(string message)
    {
        return new Error(Codes.PersonNotFoundCode, $"Person could not be found: {message}");
    }

    public static Error CsvFileNotFound(string filePath)
    {
        return new Error(Codes.FileNotFoundCode, $"CSV file not found at path: {filePath}");
    }

    public static Error CsvLoadingFailed(string message)
    {
        return new Error(Codes.CsvLoadingFailedCode, $"CSV file loading failed with message: {message}");
    }

    public static Error CsvParsingFailed(string message)
    {
        return new Error(Codes.CsvParsingFailedCode, $"CSV file parsing failed with message: {message}");
    }

    public static Error CsvPersonCreationFailed(string message)
    {
        return new Error(Codes.CsvPersonCreationFailedCode, $"CSV person creation failed with message: {message}");
    }

    public static Error CsvPersonAddingFailed(string message)
    {
        return new Error(Codes.CsvPersonAddingFailedCode, $"CSV person adding failed with message: {message}");
    }

    public static Error SqlPersonParsingFailed(string message)
    {
        return new Error(Codes.SqlPersonParsingFailedCode, $"SQL person parsing failed with message: {message}");
    }

    public static Error SqlPersonCreationToPersonFailed(string message)
    {
        return new Error(Codes.SqlToPersonCreationFailedCode, $"SQL person entity failed parsing to model Person with message: {message}");
    }

    public static Error SqlPersonCreationFailed(string message)
    {
        return new Error(Codes.SqlPersonCreationFailedCode, $"SQL person creation failed with message: {message}");
    }

    public static Error PersonEntityParsingFailed(string message)
    {
        return new Error(Codes.PersonEntityParsingFailedCode, $"Person entity parsing failed with message: {message}");
    }

    public static Error PersonDtoFailed(string message)
    {
        return new Error(Codes.PersonDtoFailedCode, $"DTO failed with message: {message}");
    }

    public static Error PersonEntityValidationFailed(string fieldName, int maxLength)
    {
        return new Error(
            Codes.PersonEntityValidationFailedCode,
            $"Person entity validation failed: {fieldName} exceeds maximum length of {maxLength}"
        );
    }

    public static Error PersonValidationFailed(string fieldName, int maxLength)
    {
        return new Error(Codes.PersonValidationFailedCode, $"Person validation failed: {fieldName} exceeds maximum length of {maxLength}");
    }

    public static class Codes
    {
        public const string InvalidAddressZipCodeCode = nameof(InvalidAddressZipCodeCode);
        public const string InvalidAddressCityCode = nameof(InvalidAddressCityCode);
        public const string InvalidColorCode = nameof(InvalidColorCode);
        public const string FileNotFoundCode = nameof(FileNotFoundCode);
        public const string CsvLoadingFailedCode = nameof(CsvLoadingFailedCode);
        public const string CsvParsingFailedCode = nameof(CsvParsingFailedCode);
        public const string AddressIsMissingCode = nameof(AddressIsMissingCode);
        public const string PersonNotFoundCode = nameof(PersonNotFoundCode);
        public const string PersonDtoFailedCode = nameof(PersonDtoFailedCode);
        public const string PersonEntityParsingFailedCode = nameof(PersonEntityParsingFailedCode);
        public const string PersonEntityValidationFailedCode = nameof(PersonEntityValidationFailedCode);
        public const string PersonValidationFailedCode = nameof(PersonValidationFailedCode);
        public const string SqlPersonParsingFailedCode = nameof(SqlPersonParsingFailedCode);
        public const string SqlToPersonCreationFailedCode = nameof(SqlToPersonCreationFailedCode);
        public const string SqlPersonCreationFailedCode = nameof(SqlPersonCreationFailedCode);
        public const string CsvPersonAddingFailedCode = nameof(CsvPersonAddingFailedCode);
        public const string CsvPersonCreationFailedCode = nameof(CsvPersonCreationFailedCode);
    }
}
