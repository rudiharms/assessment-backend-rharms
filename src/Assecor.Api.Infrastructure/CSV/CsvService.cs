using System.Globalization;
using System.IO.Abstractions;
using Assecor.Api.Domain.Common;
using Assecor.Api.Infrastructure.Abstractions;
using Assecor.Api.Infrastructure.Options;
using CSharpFunctionalExtensions;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Assecor.Api.Infrastructure.Csv;

/// <summary>
///     For the time being we accept the fact that the csv won't be changed during run-time, so we load it once and cache
///     the result.
///     In case the file should change during run-time, something similar to a FileSystemWatcher could be implemented to
///     refresh the cache.
/// </summary>
public class CsvService : ICsvService
{
    private readonly string _delimiter;
    private readonly string _filePath;
    private readonly IFileSystem _fileSystem;
    private readonly Lazy<Result<IEnumerable<CsvPerson>, Error>> _lazyData;
    private readonly ILogger<CsvService> _logger;

    public CsvService(IOptionsMonitor<CsvOptions> csvOptions, IFileSystem fileSystem, ILogger<CsvService> logger)
    {
        _filePath = csvOptions.CurrentValue.FilePath;
        _delimiter = csvOptions.CurrentValue.Delimiter;
        _fileSystem = fileSystem;
        _logger = logger;
        _lazyData = new Lazy<Result<IEnumerable<CsvPerson>, Error>>(LoadData);
    }

    public Task<Result<IEnumerable<CsvPerson>, Error>> GetDataAsync()
    {
        return Task.FromResult(_lazyData.Value);
    }

    private Result<IEnumerable<CsvPerson>, Error> LoadData()
    {
        try
        {
            if (!_fileSystem.File.Exists(_filePath))
            {
                return Errors.FileNotFound(_filePath);
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                TrimOptions = TrimOptions.Trim,
                BadDataFound = context =>
                {
                    if (context.Context.Parser != null)
                    {
                        _logger.LogWarning(
                            "Bad data found in CSV at row {Row}, field {Field}: {RawRecord}",
                            context.Context.Parser.Row,
                            context.Field,
                            context.RawRecord
                        );
                    }
                },
                MissingFieldFound = null,
                Delimiter = _delimiter
            };

            using var stream = _fileSystem.File.OpenRead(_filePath);

            if (!stream.CanRead || stream.Length == 0)
            {
                return Errors.CsvLoadingFailed("File is not readable or empty");
            }

            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, config);

            var records = new List<CsvPerson>();

            while (csv.Read())
            {
                var record = new CsvPerson
                {
                    LastName = csv.TryGetField<string>(0, out var lastName) ? lastName : string.Empty,
                    FirstName = csv.TryGetField<string>(1, out var firstName) ? firstName : string.Empty,
                    Address = csv.TryGetField<string>(2, out var address) ? address : null,
                    ColorId = csv.TryGetField<int>(3, out var colorId) ? colorId : null
                };

                if (record.HasNullOrEmptyData() is not null)
                {
                    _logger.LogWarning("Loaded CSV record from line {Line} has missing data: {@Record}", csv.Context.Parser?.Row, record);
                }

                records.Add(record);
            }

            return records;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load CSV file from path: {FilePath}", _filePath);

            return Errors.CsvLoadingFailed(ex.Message);
        }
    }
}
