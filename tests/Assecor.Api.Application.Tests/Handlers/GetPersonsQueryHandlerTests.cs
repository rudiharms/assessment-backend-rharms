using Assecor.Api.Application.Abstractions;
using Assecor.Api.Application.Handlers;
using Assecor.Api.Application.Queries;
using Assecor.Api.Domain.Common;
using Assecor.Api.Domain.Models;
using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace Assecor.Api.Application.Tests.Handlers;

//Note for Reviewer : Basic smoke tests, not 100% coverage
public class GetPersonsQueryHandlerTests
{
    private readonly IPersonRepository _personRepository;
    private readonly GetPersonsQueryHandler _sut;

    public GetPersonsQueryHandlerTests()
    {
        _personRepository = Substitute.For<IPersonRepository>();
        _sut = new GetPersonsQueryHandler(_personRepository, new NullLogger<GetPersonsQueryHandler>());
    }

    [Fact]
    public async Task Handle_Succeeds_When_Persons_Exist()
    {
        var persons = new List<Person>
        {
            Person.Create(1, "John", "Doe", Address.Create("12345", "City").Value, Color.GetById(1).Value).Value,
            Person.Create(2, "Jane", "Smith", Address.Create("67890", "Town").Value, Color.GetById(2).Value).Value
        };

        _personRepository.GetPersonsAsync().Returns(Result.Success<IEnumerable<Person>, Error>(persons));

        var query = new GetPersonsQuery();
        var result = await _sut.Handle(query, CancellationToken.None);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
        }
    }

    [Fact]
    public async Task Handle_Succeeds_When_No_Persons_Exist()
    {
        _personRepository.GetPersonsAsync().Returns(Result.Success<IEnumerable<Person>, Error>(Enumerable.Empty<Person>()));

        var query = new GetPersonsQuery();
        var result = await _sut.Handle(query, CancellationToken.None);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task Handle_Returns_Error_When_Repository_Fails()
    {
        var error = new Error("TestError", "Test error message");
        _personRepository.GetPersonsAsync().Returns(Result.Failure<IEnumerable<Person>, Error>(error));

        var query = new GetPersonsQuery();
        var result = await _sut.Handle(query, CancellationToken.None);

        using (new AssertionScope())
        {
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be(QueryErrors.Codes.PersonsQueryFailedCode);
        }
    }
}
