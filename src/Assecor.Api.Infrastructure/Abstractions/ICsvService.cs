using Assecor.Api.Domain.Common;
using Assecor.Api.Infrastructure.CSV;
using CSharpFunctionalExtensions;

namespace Assecor.Api.Infrastructure.Abstractions;

public interface ICsvService
{
    Task<Result<IEnumerable<CsvPersonRow>, Error>> GetDataAsync();
}
