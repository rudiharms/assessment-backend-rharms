using System.Globalization;
using Assecor.Api.Domain.Common;
using Assecor.Api.Infrastructure.Abstractions;
using Assecor.Api.Infrastructure.Options;
using CSharpFunctionalExtensions;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Assecor.Api.Infrastructure.CSV;

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
    private readonly Lazy<Result<IEnumerable<CsvPersonRow>, Error>> _lazyData;
    private readonly ILogger<CsvService> _logger;

    public CsvService(IOptionsMonitor<CsvOptions> options, ILogger<CsvService> logger)
    {
        _filePath = options.CurrentValue.FilePath;
        _delimiter = options.CurrentValue.Delimiter;
        _logger = logger;
        _lazyData = new Lazy<Result<IEnumerable<CsvPersonRow>, Error>>(LoadData);
    }

    public Task<Result<IEnumerable<CsvPersonRow>, Error>> GetDataAsync()
    {
        return Task.FromResult(_lazyData.Value);
    }

    private Result<IEnumerable<CsvPersonRow>, Error> LoadData()
    {
        try
        {
            if (!File.Exists(_filePath))
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

            using var reader = new StreamReader(_filePath);
            using var csv = new CsvReader(reader, config);

            var records = new List<CsvPersonRow>();

            while (csv.Read())
            {
                var record = new CsvPersonRow
                {
                    LastName = csv.TryGetField<string>(0, out var lastName) ? lastName : string.Empty,
                    FirstName = csv.TryGetField<string>(1, out var firstName) ? firstName : string.Empty,
                    Address = csv.TryGetField<string>(2, out var address) ? address : null,
                    ColorId = csv.TryGetField<int>(3, out var colorId) ? colorId : null
                };

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
