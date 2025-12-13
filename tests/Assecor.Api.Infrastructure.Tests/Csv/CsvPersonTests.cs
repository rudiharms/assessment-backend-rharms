using Assecor.Api.Domain.Models;
using Assecor.Api.Infrastructure.Csv;
using FluentAssertions;
using FluentAssertions.Execution;

namespace Assecor.Api.Infrastructure.Tests.Csv;
//Note for Reviewer : Basic smoke tests, not 100% coverage

public class CsvPersonTests
{
    [Fact]
    public void Create_Succeeds_When_Valid_Data_Provided()
    {
        var result = CsvPerson.Create("Doe", "John", "12345 City", 1);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.LastName.Should().Be("Doe");
            result.Value.FirstName.Should().Be("John");
            result.Value.Address.Should().Be("12345 City");
            result.Value.ColorId.Should().Be(1);
        }
    }

    [Fact]
    public void Create_Trims_Values()
    {
        var result = CsvPerson.Create("  Doe  ", "  John  ", "  12345 City  ", 1);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.LastName.Should().Be("Doe");
            result.Value.FirstName.Should().Be("John");
            result.Value.Address.Should().Be("12345 City");
        }
    }

    [Fact]
    public void Create_Returns_Error_When_LastName_Empty()
    {
        var result = CsvPerson.Create("", "John", "12345 City", 1);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Returns_Error_When_FirstName_Empty()
    {
        var result = CsvPerson.Create("Doe", "", "12345 City", 1);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Returns_Error_When_Address_Empty()
    {
        var result = CsvPerson.Create("Doe", "John", "", 1);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Returns_Error_When_ColorId_Null()
    {
        var result = CsvPerson.Create("Doe", "John", "12345 City", null);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void FromPerson_Succeeds_When_Valid_Person_Provided()
    {
        var person = Person.Create(1, "John", "Doe", Address.Create("12345", "City").Value, Color.GetById(1).Value).Value;

        var result = CsvPerson.FromPerson(person);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.FirstName.Should().Be("John");
            result.Value.LastName.Should().Be("Doe");
            result.Value.Address.Should().Contain("12345");
            result.Value.Address.Should().Contain("City");
            result.Value.ColorId.Should().Be(1);
        }
    }

    [Fact]
    public void ToPerson_Succeeds_When_Valid_CsvPerson_Provided()
    {
        var csvPerson = CsvPerson.Create("Doe", "John", "12345 City", 1).Value;

        var result = csvPerson.ToPerson(1);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be(1);
            result.Value.FirstName.Should().Be("John");
            result.Value.LastName.Should().Be("Doe");
            result.Value.Address.ZipCode.Should().Be("12345");
            result.Value.Address.City.Should().Be("City");
        }
    }

    [Fact]
    public void ToPerson_Returns_Error_When_Invalid_ColorId()
    {
        var csvPerson = CsvPerson.Create("Doe", "John", "12345 City", 999).Value;

        var result = csvPerson.ToPerson(1);

        result.IsFailure.Should().BeTrue();
    }
}
