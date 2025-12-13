using Assecor.Api.Domain.Common;
using Assecor.Api.Infrastructure.Csv;
using CSharpFunctionalExtensions;

namespace Assecor.Api.Infrastructure.Abstractions;

public interface ICsvService
{
    Task<Result<IEnumerable<CsvPerson>, Error>> GetDataAsync();
    Task<Result<CsvPerson, Error>> AddPersonAsync(CsvPerson person);
}
