using System.Globalization;
using System.IO.Abstractions;
using System.Text;
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
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly ILogger<CsvService> _logger;
    private Lazy<Result<IEnumerable<CsvPerson>, Error>> _lazyData;

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
                return Errors.CsvFileNotFound(_filePath);
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
                var lastName = csv.TryGetField<string>(0, out var lastNameValue) ? lastNameValue : null;
                var firstName = csv.TryGetField<string>(1, out var firstNameValue) ? firstNameValue : null;
                var address = csv.TryGetField<string>(2, out var addressValue) ? addressValue : null;
                var colorId = csv.TryGetField<int>(3, out var colorIdValue) ? colorIdValue : (int?) null;

                var recordResult = CsvPerson.Create(lastName, firstName, address, colorId);

                if (recordResult.IsFailure)
                {
                    _logger.LogWarning(
                        "Skipping CSV record from line {Line} due to validation error: {ErrorMessage}",
                        csv.Context.Parser?.Row,
                        recordResult.Error.Message
                    );

                    continue;
                }

                records.Add(recordResult.Value);
            }

            return records;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load CSV file from path: {FilePath}", _filePath);

            return Errors.CsvLoadingFailed(ex.Message);
        }
    }

    public async Task<Result<CsvPerson, Error>> AddPersonAsync(CsvPerson person)
    {
        await _lock.WaitAsync();

        try
        {
            if (!_fileSystem.File.Exists(_filePath))
            {
                return Errors.CsvFileNotFound(_filePath);
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false, Delimiter = _delimiter };

            await using var stream = _fileSystem.File.Open(_filePath, FileMode.Open, FileAccess.ReadWrite);

            if (stream.Length > 0)
            {
                await EnsureStreamEndsWithNewLineAsync(stream);
            }

            await using var writer = new StreamWriter(stream, leaveOpen: false);
            await using var csv = new CsvWriter(writer, config);

            csv.WriteField(person.LastName);
            csv.WriteField(person.FirstName);
            csv.WriteField(person.Address);
            csv.WriteField(person.ColorId);
            await csv.NextRecordAsync();
            await writer.FlushAsync();

            _logger.LogInformation(
                "Successfully appended new person to CSV file: {FirstName} {LastName}",
                person.FirstName,
                person.LastName
            );

            // Reload the cache
            _lazyData = new Lazy<Result<IEnumerable<CsvPerson>, Error>>(LoadData);

            return person;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add person to CSV file: {FilePath}", _filePath);

            return Errors.CsvPersonAddingFailed(ex.Message);
        }
        finally
        {
            _lock.Release();
        }
    }

    private static async Task EnsureStreamEndsWithNewLineAsync(Stream stream)
    {
        stream.Seek(-1, SeekOrigin.End);

        var lastByte = new byte[1];
        await stream.ReadExactlyAsync(lastByte);

        if (lastByte[0] != 10)
        {
            stream.Seek(0, SeekOrigin.End);
            var newlineBytes = Encoding.UTF8.GetBytes(Environment.NewLine);
            await stream.WriteAsync(newlineBytes);
        }
        else
        {
            stream.Seek(0, SeekOrigin.End);
        }
    }
}
