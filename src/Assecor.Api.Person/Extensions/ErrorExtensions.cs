using Assecor.Api.Application.Commands;
using Assecor.Api.Application.Queries;
using Assecor.Api.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Assecor.Api.Person.Extensions;

public static class ErrorExtensions
{
    public static ProblemDetails ToProblemDetails(this Error error)
    {
        return error.Code switch
        {
            QueryErrors.Codes.PersonNotFoundCode => CreateError(error, "Person not found", StatusCodes.Status404NotFound),
            QueryErrors.Codes.ColorQueryFailedCode => CreateError(error, "Color Query failed", StatusCodes.Status500InternalServerError),
            QueryErrors.Codes.UnknownErrorCode => CreateError(error, "Unknown error", StatusCodes.Status500InternalServerError),
            QueryErrors.Codes.InvalidColorCode => CreateError(error, "Invalid color", StatusCodes.Status400BadRequest),
            CommandErrors.Codes.CreatePersonFailedCode => CreateError(error, "Failed to create person", StatusCodes.Status400BadRequest),
            CommandErrors.Codes.CreatePersonFailedInternalCode => CreateError(
                error,
                "Failed to create person",
                StatusCodes.Status500InternalServerError
            ),

            QueryErrors.Codes.PersonsQueryFailedCode => CreateError(
                error,
                "Persons query failed",
                StatusCodes.Status500InternalServerError
            ),

            _ => CreateError(InternalServerError(), "Unexpected error", StatusCodes.Status500InternalServerError)
        };
    }

    private static ProblemDetails CreateError(
        Error error,
        string message,
        int statusCode,
        List<KeyValuePair<string, object?>>? extensions = null
    )
    {
        var problemDetails = new ProblemDetails
        {
            Title = message,
            Detail = error.Message,
            Status = statusCode,
            Type = error.Code
        };

        return problemDetails;
    }

    public static Error InternalServerError()
    {
        return new Error("InternalServerErrorCode", "An unexpected error occurred");
    }
}
