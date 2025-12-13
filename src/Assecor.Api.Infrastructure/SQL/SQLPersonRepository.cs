using Assecor.Api.Application.Abstractions;
using Assecor.Api.Domain.Common;
using Assecor.Api.Domain.Enums;
using Assecor.Api.Domain.Models;
using Assecor.Api.Infrastructure.Sql.Entities;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Assecor.Api.Infrastructure.Sql;

public class SqlPersonRepository(PersonDbContext dbContext, ILogger<SqlPersonRepository> logger) : IPersonRepository
{
    public async Task<Result<IEnumerable<Person>, Error>> GetPersonsAsync()
    {
        try
        {
            var personEntities = await dbContext.Persons.ToListAsync();
            var persons = new List<Person>();

            foreach (var personEntity in personEntities)
            {
                var personResult = personEntity.ToPerson();

                if (personResult.IsFailure)
                {
                    return personResult.Error;
                }

                persons.Add(personResult.Value);
            }

            return persons;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while fetching persons");

            return Errors.SqlPersonParsingFailed(ex.Message);
        }
    }

    public async Task<Result<Person, Error>> GetPersonByIdAsync(int id)
    {
        try
        {
            var personEntity = await dbContext.Persons.FirstOrDefaultAsync(p => p.Id == id);

            if (personEntity is null)
            {
                return Errors.PersonNotFound($"Person with ID {id} not found");
            }

            return personEntity.ToPerson();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while fetching person by ID");

            return Errors.SqlPersonParsingFailed(ex.Message);
        }
    }

    public async Task<Result<IEnumerable<Person>, Error>> GetPersonsByColorAsync(ColorName colorName)
    {
        return await GetPersonsByColorAsync((int) colorName);
    }

    public async Task<Result<IEnumerable<Person>, Error>> GetPersonsByColorAsync(int colorId)
    {
        try
        {
            var personEntities = await dbContext.Persons.Where(p => p.ColorId == colorId).ToListAsync();

            var persons = new List<Person>();

            foreach (var personResult in personEntities.Select(static personEntity => personEntity.ToPerson()))
            {
                if (personResult.IsFailure)
                {
                    return personResult.Error;
                }

                persons.Add(personResult.Value);
            }

            return persons;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while fetching persons by color");

            return Errors.SqlPersonParsingFailed(ex.Message);
        }
    }

    public async Task<Result<Person, Error>> AddPersonAsync(Person person)
    {
        try
        {
            var personEntityResult = PersonEntity.FromPerson(person);

            if (personEntityResult.IsFailure)
            {
                logger.LogError(
                    "Failed to create person entity: {ErrorCode} - {ErrorMessage}",
                    personEntityResult.Error.Code,
                    personEntityResult.Error.Message
                );

                return personEntityResult.Error;
            }

            await dbContext.Persons.AddAsync(personEntityResult.Value);
            await dbContext.SaveChangesAsync();

            var addedPerson = personEntityResult.Value.ToPerson();

            if (addedPerson.IsFailure)
            {
                logger.LogError(
                    "Failed to convert added entity back to person: {ErrorCode} - {ErrorMessage}",
                    addedPerson.Error.Code,
                    addedPerson.Error.Message
                );

                return Errors.SqlPersonCreationToPersonFailed(addedPerson.Error.Message);
            }

            return addedPerson.Value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while adding person");

            return Errors.SqlPersonCreationFailed(ex.Message);
        }
    }
}
