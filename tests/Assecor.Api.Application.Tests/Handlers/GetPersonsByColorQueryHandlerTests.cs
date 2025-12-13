using Assecor.Api.Application.Abstractions;
using Assecor.Api.Application.Handlers;
using Assecor.Api.Application.Queries;
using Assecor.Api.Domain.Common;
using Assecor.Api.Domain.Enums;
using Assecor.Api.Domain.Models;
using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace Assecor.Api.Application.Tests.Handlers;

//Note for Reviewer : Basic smoke tests, not 100% coverage
public class GetPersonsByColorQueryHandlerTests
{
    private readonly IPersonRepository _personRepository;
    private readonly GetPersonsByColorQueryHandler _sut;

    public GetPersonsByColorQueryHandlerTests()
    {
        _personRepository = Substitute.For<IPersonRepository>();
        _sut = new GetPersonsByColorQueryHandler(_personRepository, new NullLogger<GetPersonsByColorQueryHandler>());
    }

    [Theory]
    [InlineData(nameof(ColorName.Blau))]
    [InlineData(nameof(ColorName.Grün))]
    [InlineData(nameof(ColorName.Rot))]
    public async Task Handle_Succeeds_When_Valid_Color_Provided(string colorName)
    {
        var persons = new List<Person>
        {
            Person.Create(1, "John", "Doe", Address.Create("12345", "City").Value, Color.GetById(1).Value).Value
        };

        _personRepository.GetPersonsByColorAsync(Arg.Any<ColorName>()).Returns(Result.Success<IEnumerable<Person>, Error>(persons));

        var query = new GetPersonsByColorQuery(colorName);
        var result = await _sut.Handle(query, CancellationToken.None);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
        }
    }

    [Fact]
    public async Task Handle_Returns_Error_When_Invalid_Color_Provided()
    {
        var query = new GetPersonsByColorQuery("InvalidColor");
        var result = await _sut.Handle(query, CancellationToken.None);

        using (new AssertionScope())
        {
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be(QueryErrors.Codes.InvalidColorCode);
        }
    }

    [Fact]
    public async Task Handle_Returns_Error_When_Repository_Fails()
    {
        var error = new Error("TestError", "Test error message");
        _personRepository.GetPersonsByColorAsync(Arg.Any<ColorName>()).Returns(Result.Failure<IEnumerable<Person>, Error>(error));

        var query = new GetPersonsByColorQuery(nameof(ColorName.Blau));
        var result = await _sut.Handle(query, CancellationToken.None);

        using (new AssertionScope())
        {
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be(QueryErrors.Codes.ColorQueryFailedCode);
        }
    }
}
