using Assecor.Api.Application.Extensions;
using Assecor.Api.Domain.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging.Abstractions;

namespace Assecor.Api.Application.Tests.Extensions;
//Note for Reviewer : Basic smoke tests, not 100% coverage

public class PersonExtensionsTests
{
    [Fact]
    public void ToPersonDto_Succeeds_When_Person_Is_Valid()
    {
        var person = Person.Create(1, "John", "Doe", Address.Create("12345", "City").Value, Color.GetById(1).Value).Value;

        var result = person.ToPersonDto();

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be(1);
            result.Value.Name.Should().Be("John");
            result.Value.LastName.Should().Be("Doe");
            result.Value.ZipCode.Should().Be("12345");
            result.Value.City.Should().Be("City");
            result.Value.Color.Should().Be("blau");
        }
    }

    [Fact]
    public void ToPersonDtos_Succeeds_When_Persons_Are_Valid()
    {
        var persons = new List<Person>
        {
            Person.Create(1, "John", "Doe", Address.Create("12345", "City").Value, Color.GetById(1).Value).Value,
            Person.Create(2, "Jane", "Smith", Address.Create("67890", "Town").Value, Color.GetById(2).Value).Value
        };

        var result = persons.ToPersonDtos(new NullLogger<PersonExtensionsTests>()).ToList();

        using (new AssertionScope())
        {
            result.Should().HaveCount(2);
            result.First().Name.Should().Be("John");
            result.Last().Name.Should().Be("Jane");
        }
    }
}
