using Assecor.Api.Domain.Models;
using Assecor.Api.Infrastructure.Sql.Entities;
using FluentAssertions;
using FluentAssertions.Execution;

namespace Assecor.Api.Infrastructure.Tests.Sql;
//Note for Reviewer : Basic smoke tests, not 100% coverage

public class PersonEntityTests
{
    [Fact]
    public void Create_Succeeds_When_Valid_Data_Provided()
    {
        var result = PersonEntity.Create("John", "Doe", "12345", "City", 1);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.FirstName.Should().Be("John");
            result.Value.LastName.Should().Be("Doe");
            result.Value.ZipCode.Should().Be("12345");
            result.Value.City.Should().Be("City");
            result.Value.ColorId.Should().Be(1);
        }
    }

    [Fact]
    public void Create_Trims_Values()
    {
        var result = PersonEntity.Create("  John  ", "  Doe  ", "  12345  ", "  City  ", 1);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.FirstName.Should().Be("John");
            result.Value.LastName.Should().Be("Doe");
            result.Value.ZipCode.Should().Be("12345");
            result.Value.City.Should().Be("City");
        }
    }

    [Fact]
    public void Create_Returns_Error_When_FirstName_Too_Long()
    {
        var longName = new string('a', 201);

        var result = PersonEntity.Create(longName, "Doe", "12345", "City", 1);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Returns_Error_When_LastName_Too_Long()
    {
        var longName = new string('a', 201);

        var result = PersonEntity.Create("John", longName, "12345", "City", 1);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Returns_Error_When_ZipCode_Too_Long()
    {
        var longZipCode = new string('1', 21);

        var result = PersonEntity.Create("John", "Doe", longZipCode, "City", 1);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Returns_Error_When_City_Too_Long()
    {
        var longCity = new string('a', 201);

        var result = PersonEntity.Create("John", "Doe", "12345", longCity, 1);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void FromPerson_Succeeds_When_Valid_Person_Provided()
    {
        var person = Person.Create(
            1,
            "John",
            "Doe",
            Address.Create("12345", "City").Value,
            Color.GetById(1).Value
        ).Value;

        var result = PersonEntity.FromPerson(person);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be(1);
            result.Value.FirstName.Should().Be("John");
            result.Value.LastName.Should().Be("Doe");
            result.Value.ZipCode.Should().Be("12345");
            result.Value.City.Should().Be("City");
            result.Value.ColorId.Should().Be(1);
        }
    }

    [Fact]
    public void ToPerson_Succeeds_When_Valid_Entity()
    {
        var entity = PersonEntity.Create("John", "Doe", "12345", "City", 1, 1).Value;

        var result = entity.ToPerson();

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
        var entity = PersonEntity.Create("John", "Doe", "12345", "City", 999, 1).Value;

        var result = entity.ToPerson();

        result.IsFailure.Should().BeTrue();
    }
}
