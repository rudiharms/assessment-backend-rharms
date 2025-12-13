using Assecor.Api.Domain.Models;
using FluentAssertions;
using FluentAssertions.Execution;

namespace Assecor.Api.Domain.Tests.Models;
//Note for Reviewer : Basic smoke tests, not 100% coverage

public class PersonTests
{
    [Fact]
    public void Create_Succeeds_When_Valid_Data_Provided()
    {
        var address = Address.Create("12345", "City").Value;
        var color = Color.GetById(1).Value;

        var result = Person.Create(1, "John", "Doe", address, color);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be(1);
            result.Value.FirstName.Should().Be("John");
            result.Value.LastName.Should().Be("Doe");
            result.Value.Address.Should().Be(address);
            result.Value.Color.Should().Be(color);
        }
    }

    [Fact]
    public void Create_Trims_Names()
    {
        var address = Address.Create("12345", "City").Value;
        var color = Color.GetById(1).Value;

        var result = Person.Create(1, "  John  ", "  Doe  ", address, color);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.FirstName.Should().Be("John");
            result.Value.LastName.Should().Be("Doe");
        }
    }

    [Fact]
    public void Create_Returns_Error_When_FirstName_Too_Long()
    {
        var address = Address.Create("12345", "City").Value;
        var color = Color.GetById(1).Value;
        var longName = new string('a', 201);

        var result = Person.Create(1, longName, "Doe", address, color);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Returns_Error_When_LastName_Too_Long()
    {
        var address = Address.Create("12345", "City").Value;
        var color = Color.GetById(1).Value;
        var longName = new string('a', 201);

        var result = Person.Create(1, "John", longName, address, color);

        result.IsFailure.Should().BeTrue();
    }
}
