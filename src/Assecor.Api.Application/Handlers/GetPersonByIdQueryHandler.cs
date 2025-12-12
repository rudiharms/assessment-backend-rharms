using Assecor.Api.Application.Abstractions;
using Assecor.Api.Application.DTOs;
using Assecor.Api.Application.Extensions;
using Assecor.Api.Application.Queries;
using Assecor.Api.Domain.Common;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Assecor.Api.Application.Handlers;

public class GetPersonByIdQueryHandler(IPersonRepository personRepository, ILogger<GetPersonByIdQueryHandler> logger)
    : IRequestHandler<GetPersonByIdQuery, Result<PersonDto, Error>>
{
    public async Task<Result<PersonDto, Error>> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
    {
        var personResult = await personRepository.GetPersonByIdAsync(request.Id);

        if (personResult.IsFailure)
        {
            logger.LogError(
                "Failed to get person by Id {PersonId}: {ErrorCode} - {ErrorMessage}",
                request.Id,
                personResult.Error.Code,
                personResult.Error.Message
            );

            return QueryErrors.PersonNotFound(request.Id);
        }

        var personDto = personResult.Value.ToPersonDto();

        if (personDto.IsFailure)
        {
            logger.LogWarning(
                "Failed to get person dto by Id {PersonId}: {ErrorCode} - {ErrorMessage}",
                request.Id,
                personDto.Error.Code,
                personDto.Error.Message
            );

            return QueryErrors.UnknownError(personDto.Error.Message);
        }

        return personDto;
    }
}
