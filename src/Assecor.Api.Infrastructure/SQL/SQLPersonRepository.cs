using Assecor.Api.Application.Abstractions;
using Assecor.Api.Application.DTOs;
using Assecor.Api.Domain.Common;
using CSharpFunctionalExtensions;

namespace Assecor.Api.Infrastructure.SQL;

public class SQLPersonRepository : IPersonRepository
{
    public Task<Result<IEnumerable<PersonDto>, Error>> GetPersonsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result<PersonDto, Error>> GetPersonByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<PersonDto>, Error>> GetPersonsByColorAsync(string color)
    {
        throw new NotImplementedException();
    }
}
