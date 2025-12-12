using Assecor.Api.Application.Abstractions;
using Assecor.Api.Application.DTOs;
using Assecor.Api.Application.Extensions;
using Assecor.Api.Application.Queries;
using Assecor.Api.Domain.Common;
using Assecor.Api.Domain.Enums;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Assecor.Api.Application.Handlers;

public class GetPersonsByColorQueryHandler(IPersonRepository personRepository, ILogger<GetPersonsByColorQueryHandler> logger)
    : IRequestHandler<GetPersonsByColorQuery, Result<IEnumerable<PersonDto>, Error>>
{
    public async Task<Result<IEnumerable<PersonDto>, Error>> Handle(GetPersonsByColorQuery request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<ColorName>(request.ColorName, true, out var colorEnum))
        {
            logger.LogWarning("Invalid color requested {ColorName}", request.ColorName);

            return QueryErrors.InvalidColor(request.ColorName);
        }

        var personsResult = await personRepository.GetPersonsByColorAsync(colorEnum);

        if (personsResult.IsFailure)
        {
            logger.LogError(
                "Failed to get persons for color {ColorName}: {ErrorCode} - {ErrorMessage}",
                request.ColorName,
                personsResult.Error.Code,
                personsResult.Error.Message
            );

            return QueryErrors.ColorQueryFailed(colorEnum);
        }

        var personDtos = personsResult.Value.ToPersonDtos(logger);

        return Result.Success<IEnumerable<PersonDto>, Error>(personDtos);
    }
}
