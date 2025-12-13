using Assecor.Api.Application.Abstractions;
using Assecor.Api.Domain.Common;
using Assecor.Api.Domain.Enums;
using Assecor.Api.Domain.Models;
using Assecor.Api.Infrastructure.Abstractions;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;

namespace Assecor.Api.Infrastructure.Csv;

public class CsvPersonRepository(ICsvService csvService, ILogger<CsvPersonRepository> logger) : IPersonRepository
{
    public async Task<Result<IEnumerable<Person>, Error>> GetPersonsAsync()
    {
        var csvDataResult = await csvService.GetDataAsync();

        if (csvDataResult.IsFailure)
        {
            return csvDataResult.Error;
        }

        var persons = new List<Person>();

        var personId = 1;

        foreach (var row in csvDataResult.Value)
        {
            var personResult = row.ToPerson(personId);

            if (personResult.IsFailure)
            {
                logger.LogWarning("Failed to convert CSV row to Person: {Error}", personResult.Error.Message);

                continue;
            }

            persons.Add(personResult.Value);
            personId++;
        }

        return persons;
    }

    public async Task<Result<Person, Error>> GetPersonByIdAsync(int id)
    {
        var personsResult = await GetPersonsAsync();

        if (personsResult.IsFailure)
        {
            return personsResult.Error;
        }

        var person = personsResult.Value.FirstOrDefault(person => person.Id == id);

        if (person is null)
        {
            return Errors.PersonNotFound($"Person with ID {id} not found.");
        }

        return person;
    }

    public async Task<Result<IEnumerable<Person>, Error>> GetPersonsByColorAsync(ColorName colorName)
    {
        var personsResult = await GetPersonsAsync();

        if (personsResult.IsFailure)
        {
            return personsResult.Error;
        }

        var filteredPersons = personsResult.Value.Where(p => p.Color.ColorName == colorName).ToList();

        return filteredPersons;
    }

    public async Task<Result<IEnumerable<Person>, Error>> GetPersonsByColorAsync(int colorId)
    {
        var personsResult = await GetPersonsAsync();

        if (personsResult.IsFailure)
        {
            return personsResult.Error;
        }

        var filteredPersons = personsResult.Value.Where(p => p.Color.Id == colorId).ToList();

        return filteredPersons;
    }

    public async Task<Result<Person, Error>> AddPersonAsync(Person person)
    {
        try
        {
            var personsResult = await GetPersonsAsync();

            if (personsResult.IsFailure)
            {
                return personsResult.Error;
            }

            var nextId = personsResult.Value.Any() ? personsResult.Value.Max(static p => p.Id) + 1 : 1;

            var csvPersonResult = CsvPerson.FromPerson(person);

            if (csvPersonResult.IsFailure)
            {
                logger.LogError(
                    "Failed to create CSV person: {ErrorCode} - {ErrorMessage}",
                    csvPersonResult.Error.Code,
                    csvPersonResult.Error.Message
                );

                return csvPersonResult.Error;
            }

            var addResult = await csvService.AddPersonAsync(csvPersonResult.Value);

            if (addResult.IsFailure)
            {
                logger.LogError("Failed to add person to CSV: {ErrorCode} - {ErrorMessage}", addResult.Error.Code, addResult.Error.Message);

                return addResult.Error;
            }

            var addedPerson = Person.Create(nextId, person.FirstName, person.LastName, person.Address, person.Color);

            if (addedPerson.IsFailure)
            {
                logger.LogError(
                    "Person saved to CSV but failed to create domain model: {ErrorCode} - {ErrorMessage}",
                    addedPerson.Error.Code,
                    addedPerson.Error.Message
                );

                return Errors.SqlPersonCreationToPersonFailed(addedPerson.Error.Message);
            }

            logger.LogInformation("Successfully added person with ID {PersonId} to CSV repository", nextId);

            return addedPerson.Value;
        }

        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while adding person to CSV repository");

            return Errors.CsvLoadingFailed(ex.Message);
        }
    }
}
