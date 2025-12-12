using Assecor.Api.Application.Abstractions;
using Assecor.Api.Application.DTOs;
using Assecor.Api.Application.Extensions;
using Assecor.Api.Application.Queries;
using Assecor.Api.Domain.Common;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Assecor.Api.Application.Handlers;

public class GetPersonsQueryHandler(IPersonRepository personRepository, ILogger<GetPersonsQueryHandler> logger)
    : IRequestHandler<GetPersonsQuery, Result<IEnumerable<PersonDto>, Error>>
{
    public async Task<Result<IEnumerable<PersonDto>, Error>> Handle(GetPersonsQuery request, CancellationToken cancellationToken)
    {
        var personsResult = await personRepository.GetPersonsAsync();

        if (personsResult.IsFailure)
        {
            logger.LogError("Failed to get persons: {ErrorCode} - {ErrorMessage}", personsResult.Error.Code, personsResult.Error.Message);

            return QueryErrors.PersonsQueryFailed();
        }

        var personDtos = personsResult.Value.ToPersonDtos(logger);

        return Result.Success<IEnumerable<PersonDto>, Error>(personDtos);
    }
}
