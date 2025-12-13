using Assecor.Api.Domain.Common;
using Assecor.Api.Domain.Enums;
using Assecor.Api.Domain.Models;
using Assecor.Api.Infrastructure.Abstractions;
using Assecor.Api.Infrastructure.Csv;
using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace Assecor.Api.Infrastructure.Tests.Csv;
//Note for Reviewer : Basic smoke tests, not 100% coverage

public class CsvPersonRepositoryTests
{
    private readonly ICsvService _csvService;
    private readonly CsvPersonRepository _sut;

    public CsvPersonRepositoryTests()
    {
        _csvService = Substitute.For<ICsvService>();
        _sut = new CsvPersonRepository(_csvService, new NullLogger<CsvPersonRepository>());
    }

    [Fact]
    public async Task GetPersonsAsync_Succeeds_When_Csv_Data_Valid()
    {
        var csvPersons = new List<CsvPerson>
        {
            CsvPerson.Create("Doe", "John", "12345 City", 1).Value, CsvPerson.Create("Smith", "Jane", "67890 Town", 2).Value
        };

        _csvService.GetDataAsync().Returns(Result.Success<IEnumerable<CsvPerson>, Error>(csvPersons));

        var result = await _sut.GetPersonsAsync();

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
            result.Value.First().FirstName.Should().Be("John");
            result.Value.Last().FirstName.Should().Be("Jane");
        }
    }

    [Fact]
    public async Task GetPersonsAsync_Returns_Error_When_Csv_Service_Fails()
    {
        var error = new Error("TestError", "Test error");
        _csvService.GetDataAsync().Returns(Result.Failure<IEnumerable<CsvPerson>, Error>(error));

        var result = await _sut.GetPersonsAsync();

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task GetPersonByIdAsync_Succeeds_When_Person_Exists()
    {
        var csvPersons = new List<CsvPerson>
        {
            CsvPerson.Create("Doe", "John", "12345 City", 1).Value, CsvPerson.Create("Smith", "Jane", "67890 Town", 2).Value
        };

        _csvService.GetDataAsync().Returns(Result.Success<IEnumerable<CsvPerson>, Error>(csvPersons));

        var result = await _sut.GetPersonByIdAsync(2);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be(2);
            result.Value.FirstName.Should().Be("Jane");
        }
    }

    [Fact]
    public async Task GetPersonByIdAsync_Returns_Error_When_Person_Not_Found()
    {
        var csvPersons = new List<CsvPerson> { CsvPerson.Create("Doe", "John", "12345 City", 1).Value };

        _csvService.GetDataAsync().Returns(Result.Success<IEnumerable<CsvPerson>, Error>(csvPersons));

        var result = await _sut.GetPersonByIdAsync(999);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task GetPersonsByColorAsync_By_ColorName_Succeeds()
    {
        var csvPersons = new List<CsvPerson>
        {
            CsvPerson.Create("Doe", "John", "12345 City", 1).Value,
            CsvPerson.Create("Smith", "Jane", "67890 Town", 1).Value,
            CsvPerson.Create("Brown", "Bob", "11111 Place", 2).Value
        };

        _csvService.GetDataAsync().Returns(Result.Success<IEnumerable<CsvPerson>, Error>(csvPersons));

        var result = await _sut.GetPersonsByColorAsync(ColorName.Blau);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
            result.Value.Should().AllSatisfy(static p => p.Color.ColorName.Should().Be(ColorName.Blau));
        }
    }

    [Fact]
    public async Task GetPersonsByColorAsync_By_ColorId_Succeeds()
    {
        var csvPersons = new List<CsvPerson>
        {
            CsvPerson.Create("Doe", "John", "12345 City", 1).Value, CsvPerson.Create("Smith", "Jane", "67890 Town", 2).Value
        };

        _csvService.GetDataAsync().Returns(Result.Success<IEnumerable<CsvPerson>, Error>(csvPersons));

        var result = await _sut.GetPersonsByColorAsync(1);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
            result.Value.First().Color.Id.Should().Be(1);
        }
    }

    [Fact]
    public async Task AddPersonAsync_Succeeds_When_Person_Valid()
    {
        var existingCsvPersons = new List<CsvPerson> { CsvPerson.Create("Doe", "John", "12345 City", 1).Value };

        _csvService.GetDataAsync().Returns(Result.Success<IEnumerable<CsvPerson>, Error>(existingCsvPersons));
        _csvService.AddPersonAsync(Arg.Any<CsvPerson>()).Returns(callInfo => Result.Success<CsvPerson, Error>(callInfo.Arg<CsvPerson>()));

        var person = Person.Create(0, "Jane", "Smith", Address.Create("67890", "Town").Value, Color.GetById(2).Value).Value;

        var result = await _sut.AddPersonAsync(person);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be(2);
            result.Value.FirstName.Should().Be("Jane");
            await _csvService.Received(1).AddPersonAsync(Arg.Any<CsvPerson>());
        }
    }

    [Fact]
    public async Task AddPersonAsync_Returns_Error_When_Csv_Service_Fails()
    {
        var existingCsvPersons = new List<CsvPerson> { CsvPerson.Create("Doe", "John", "12345 City", 1).Value };

        var error = new Error("TestError", "Test error");
        _csvService.GetDataAsync().Returns(Result.Success<IEnumerable<CsvPerson>, Error>(existingCsvPersons));
        _csvService.AddPersonAsync(Arg.Any<CsvPerson>()).Returns(Result.Failure<CsvPerson, Error>(error));

        var person = Person.Create(0, "Jane", "Smith", Address.Create("67890", "Town").Value, Color.GetById(2).Value).Value;

        var result = await _sut.AddPersonAsync(person);

        result.IsFailure.Should().BeTrue();
    }
}
