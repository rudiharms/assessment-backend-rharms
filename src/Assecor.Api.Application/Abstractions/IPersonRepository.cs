using Assecor.Api.Domain.Common;
using Assecor.Api.Domain.Enums;
using Assecor.Api.Domain.Models;
using CSharpFunctionalExtensions;

namespace Assecor.Api.Application.Abstractions;

public interface IPersonRepository
{
    Task<Result<IEnumerable<Person>, Error>> GetPersonsAsync();
    Task<Result<Person, Error>> GetPersonByIdAsync(int id);
    Task<Result<IEnumerable<Person>, Error>> GetPersonsByColorAsync(ColorName colorName);
    Task<Result<IEnumerable<Person>, Error>> GetPersonsByColorAsync(int colorId);
    Task<Result<Person, Error>> AddPersonAsync(Person person);
}
