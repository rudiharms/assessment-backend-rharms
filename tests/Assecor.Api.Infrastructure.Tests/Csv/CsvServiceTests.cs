using System.IO.Abstractions.TestingHelpers;
using Assecor.Api.Infrastructure.Csv;
using Assecor.Api.Infrastructure.Options;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Assecor.Api.Infrastructure.Tests.Csv;
//Note for Reviewer : Basic smoke tests, not 100% coverage

public class CsvServiceTests
{
    private const string TestFilePath = @"C:\test\sample.csv";
    private readonly MockFileSystem _fileSystem = new();

    [Fact]
    public async Task GetDataAsync_Succeeds_When_Csv_File_Valid()
    {
        var csvContent = """
                         Müller,Hans,67742 Lauterecken,1
                         Schmidt,Peter,55232 Alzey,2
                         """;

        _fileSystem.AddFile(TestFilePath, new MockFileData(csvContent));

        var options = CreateOptions(TestFilePath);
        var sut = new CsvService(options, _fileSystem, new NullLogger<CsvService>());

        var result = await sut.GetDataAsync();

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
            result.Value.First().LastName.Should().Be("Müller");
            result.Value.First().FirstName.Should().Be("Hans");
        }
    }

    [Fact]
    public async Task GetDataAsync_Returns_Error_When_File_Not_Found()
    {
        var options = CreateOptions(TestFilePath);
        var sut = new CsvService(options, _fileSystem, new NullLogger<CsvService>());

        var result = await sut.GetDataAsync();

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task GetDataAsync_Returns_Succeeds_When_File_Empty()
    {
        _fileSystem.AddFile(TestFilePath, new MockFileData(string.Empty));

        var options = CreateOptions(TestFilePath);
        var sut = new CsvService(options, _fileSystem, new NullLogger<CsvService>());

        var result = await sut.GetDataAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDataAsync_Skips_Invalid_Rows()
    {
        var csvContent = """
                         Müller,Hans,67742 Lauterecken,1
                         ,,InvalidRow,
                         Schmidt,Peter,55232 Alzey,2
                         """;

        _fileSystem.AddFile(TestFilePath, new MockFileData(csvContent));

        var options = CreateOptions(TestFilePath);
        var sut = new CsvService(options, _fileSystem, new NullLogger<CsvService>());

        var result = await sut.GetDataAsync();

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
        }
    }

    [Fact]
    public async Task AddPersonAsync_Succeeds_When_Person_Valid()
    {
        var csvContent = """
                         Müller,Hans,67742 Lauterecken,1
                         Schmidt,Peter,55232 Alzey,2
                         """;

        _fileSystem.AddFile(TestFilePath, new MockFileData(csvContent));

        var options = CreateOptions(TestFilePath);
        var sut = new CsvService(options, _fileSystem, new NullLogger<CsvService>());

        var newPerson = CsvPerson.Create("Brown", "Bob", "11111 Place", 3).Value;

        var result = await sut.AddPersonAsync(newPerson);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.LastName.Should().Be("Brown");

            var fileContent = await _fileSystem.File.ReadAllTextAsync(TestFilePath);
            fileContent.Should().Contain("Brown");
            fileContent.Should().Contain("Bob");
        }
    }

    [Fact]
    public async Task AddPersonAsync_Returns_Error_When_File_Not_Found()
    {
        var options = CreateOptions(TestFilePath);
        var sut = new CsvService(options, _fileSystem, new NullLogger<CsvService>());

        var newPerson = CsvPerson.Create("Brown", "Bob", "11111 Place", 3).Value;

        var result = await sut.AddPersonAsync(newPerson);

        result.IsFailure.Should().BeTrue();
    }

    private static IOptionsMonitor<CsvOptions> CreateOptions(string filePath)
    {
        var options = new CsvOptions { FilePath = filePath, Delimiter = "," };
        var mock = Substitute.For<IOptionsMonitor<CsvOptions>>();
        mock.CurrentValue.Returns(options);

        return mock;
    }
}
