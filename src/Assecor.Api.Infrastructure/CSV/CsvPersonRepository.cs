using Assecor.Api.Application.Abstractions;
using Assecor.Api.Application.DTOs;
using Assecor.Api.Domain.Common;
using Assecor.Api.Domain.Models;
using Assecor.Api.Infrastructure.Abstractions;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;

namespace Assecor.Api.Infrastructure.CSV;

public class CsvPersonRepository(ICsvService csvService, ILogger<CsvPersonRepository> logger) : IPersonRepository
{
    public async Task<Result<IEnumerable<PersonDto>, Error>> GetPersonsAsync()
    {
        var csvDataResult = await csvService.GetDataAsync();

        if (csvDataResult.IsFailure)
        {
            return csvDataResult.Error;
        }

        var persons = new List<PersonDto>();

        var personId = 1;

        foreach (var row in csvDataResult.Value)
        {
            var personResult = row.ToPersonDto(personId);

            if (personResult.IsFailure)
            {
                logger.LogWarning("Failed to convert CSV row to PersonDto: {Error}", personResult.Error.Message);

                continue;
            }

            persons.Add(personResult.Value);
            personId++;
        }

        return persons;
    }

    public async Task<Result<PersonDto, Error>> GetPersonByIdAsync(int id)
    {
        var personsResult = await GetPersonsAsync();

        if (personsResult.IsFailure)
        {
            return personsResult.Error;
        }

        var person = personsResult.Value.Skip(id - 1).FirstOrDefault();

        if (person == null)
        {
            return Result.Failure<PersonDto, Error>(new Error("PersonNotFound", $"Person with ID {id} not found."));
        }

        return Result.Success<PersonDto, Error>(person);
    }

    public async Task<Result<IEnumerable<PersonDto>, Error>> GetPersonsByColorAsync(string color)
    {
        var personsResult = await GetPersonsAsync();

        if (personsResult.IsFailure)
        {
            return Result.Failure<IEnumerable<PersonDto>, Error>(personsResult.Error);
        }

        var filteredPersons = personsResult.Value.Where(p => p.Color?.Id.ToString() == color ||
                                                             string.Equals(
                                                                 GetColorNameById(p.Color?.Id),
                                                                 color,
                                                                 StringComparison.OrdinalIgnoreCase
                                                             )
            )
            .ToList();

        return Result.Success<IEnumerable<PersonDto>, Error>(filteredPersons);
    }

    private string? GetColorNameById(int? colorId)
    {
        if (!colorId.HasValue)
        {
            return null;
        }

        var colorResult = Color.GetById(colorId.Value);

        return colorResult.IsSuccess ? colorResult.Value.Name : null;
    }
}
