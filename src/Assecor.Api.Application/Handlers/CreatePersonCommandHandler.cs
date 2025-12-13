using Assecor.Api.Application.Abstractions;
using Assecor.Api.Application.Commands;
using Assecor.Api.Application.DTOs;
using Assecor.Api.Application.Extensions;
using Assecor.Api.Domain.Common;
using Assecor.Api.Domain.Models;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Assecor.Api.Application.Handlers;

public class CreatePersonCommandHandler(IPersonRepository personRepository, ILogger<CreatePersonCommandHandler> logger)
    : IRequestHandler<CreatePersonCommand, Result<PersonDto, Error>>
{
    public async Task<Result<PersonDto, Error>> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        var dto = request.PersonDto;

        var addressResult = Address.Create(dto.ZipCode, dto.City);

        if (addressResult.IsFailure)
        {
            logger.LogWarning(
                "Failed to create address: {ErrorCode} - {ErrorMessage}",
                addressResult.Error.Code,
                addressResult.Error.Message
            );

            return CommandErrors.CreatePersonFailed(addressResult.Error.Message);
        }

        var colorResult = Color.GetByName(dto.Color);

        if (colorResult.IsFailure)
        {
            logger.LogWarning(
                "Failed to parse color '{Color}': {ErrorCode} - {ErrorMessage}",
                dto.Color,
                colorResult.Error.Code,
                colorResult.Error.Message
            );

            return CommandErrors.CreatePersonFailed(colorResult.Error.Message);
        }

        var personResult = Person.Create(0, dto.Name, dto.LastName, addressResult.Value, colorResult.Value);

        if (personResult.IsFailure)
        {
            logger.LogWarning("Failed to create person: {ErrorCode} - {ErrorMessage}", personResult.Error.Code, personResult.Error.Message);

            return CommandErrors.CreatePersonFailed(personResult.Error.Message);
        }

        var addResult = await personRepository.AddPersonAsync(personResult.Value);

        if (addResult.IsFailure)
        {
            logger.LogError(
                "Failed to add person to repository: {ErrorCode} - {ErrorMessage}",
                addResult.Error.Code,
                addResult.Error.Message
            );

            if (addResult.Error.Code == Errors.Codes.SqlToPersonCreationFailedCode)
            {
                return CommandErrors.CreatePersonFailedPostPersistence(addResult.Error.Message);
            }

            return CommandErrors.CreatePersonFailedInternal(addResult.Error.Message);
        }

        var personDtoResult = addResult.Value.ToPersonDto();

        if (personDtoResult.IsFailure)
        {
            logger.LogWarning(
                "Failed to convert person to DTO: {ErrorCode} - {ErrorMessage}",
                personDtoResult.Error.Code,
                personDtoResult.Error.Message
            );

            return CommandErrors.CreatePersonFailedPostPersistence(personDtoResult.Error.Message);
        }

        return personDtoResult.Value;
    }
}
