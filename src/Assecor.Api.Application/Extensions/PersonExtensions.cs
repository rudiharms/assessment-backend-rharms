using Assecor.Api.Application.DTOs;
using Assecor.Api.Domain.Common;
using Assecor.Api.Domain.Models;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;

namespace Assecor.Api.Application.Extensions;

public static class PersonExtensions
{
    public static Result<PersonDto, Error> ToPersonDto(this Person person)
    {
        try
        {
            return new PersonDto(
                person.Id, 
                person.FirstName, 
                person.LastName, 
                person.Address.ZipCode,
                person.Address.City, 
                person.Color.ColorName.ToString().ToLowerInvariant());
        }
        catch (Exception e)
        {
            return Errors.PersonQueryFailed(e.Message);
        }
    }

    public static IEnumerable<PersonDto> ToPersonDtos(this IEnumerable<Person> persons, ILogger logger)
    {
        return persons
            .Select(person =>
            {
                var personDto = person.ToPersonDto();

                if (!personDto.IsFailure)
                {
                    return personDto.Value;
                }

                logger.LogError(
                    "Failed to get person dto by id {PersonId}: {ErrorCode} - {ErrorMessage}",
                    person.Id,
                    personDto.Error.Code,
                    personDto.Error.Message
                );

                return null;
            })
            .OfType<PersonDto>();
    }
}
