using Assecor.Api.Domain.Common;

namespace Assecor.Api.Application.Commands;

public static class CommandErrors
{
    public static Error CreatePersonFailed(string message)
    {
        return new Error(Codes.CreatePersonFailedCode, $"Person could not be created with error: {message}");
    }

    public static Error CreatePersonFailedInternal(string message)
    {
        return new Error(Codes.CreatePersonFailedInternalCode, $"Person could not be created with error: {message}");
    }

    public static Error CreatePersonFailedPostPersistence(string message)
    {
        return new Error(Codes.CreatePersonFailedPostPersistenceCode, $"Person was created, but could not be retrieved: {message}");
    }

    public static class Codes
    {
        public const string CreatePersonFailedCode = nameof(CreatePersonFailedCode);
        public const string CreatePersonFailedPostPersistenceCode = nameof(CreatePersonFailedPostPersistenceCode);
        public const string CreatePersonFailedInternalCode = nameof(CreatePersonFailedInternalCode);
    }
}
