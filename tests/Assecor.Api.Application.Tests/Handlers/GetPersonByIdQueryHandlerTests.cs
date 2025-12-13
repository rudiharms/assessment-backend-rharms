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
public class GetPersonByIdQueryHandlerTests
{
    private readonly IPersonRepository _personRepository;
    private readonly GetPersonByIdQueryHandler _sut;

    public GetPersonByIdQueryHandlerTests()
    {
        _personRepository = Substitute.For<IPersonRepository>();
        _sut = new GetPersonByIdQueryHandler(_personRepository, new NullLogger<GetPersonByIdQueryHandler>());
    }

    [Fact]
    public async Task Handle_Succeeds_When_Person_Exists()
    {
        var person = Person.Create(1, "John", "Doe", Address.Create("12345", "City").Value, Color.GetById(1).Value).Value;

        _personRepository.GetPersonByIdAsync(1).Returns(Result.Success<Person, Error>(person));

        var query = new GetPersonByIdQuery(1);
        var result = await _sut.Handle(query, CancellationToken.None);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be(1);
            result.Value.Name.Should().Be("John");
            result.Value.ZipCode.Should().Be("12345");
            result.Value.LastName.Should().Be("Doe");
        }
    }

    [Fact]
    public async Task Handle_Returns_Error_When_Person_Not_Found()
    {
        var error = new Error("PersonNotFound", "Person not found");
        _personRepository.GetPersonByIdAsync(999).Returns(Result.Failure<Person, Error>(error));

        var query = new GetPersonByIdQuery(999);
        var result = await _sut.Handle(query, CancellationToken.None);

        using (new AssertionScope())
        {
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be(QueryErrors.Codes.PersonNotFoundCode);
        }
    }
}
