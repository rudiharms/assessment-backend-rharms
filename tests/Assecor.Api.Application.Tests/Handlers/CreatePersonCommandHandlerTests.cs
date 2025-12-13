using Assecor.Api.Application.Abstractions;
using Assecor.Api.Application.Commands;
using Assecor.Api.Application.DTOs;
using Assecor.Api.Application.Handlers;
using Assecor.Api.Domain.Common;
using Assecor.Api.Domain.Models;
using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace Assecor.Api.Application.Tests.Handlers;

//Note for Reviewer : Basic smoke tests, not 100% coverage
public class CreatePersonCommandHandlerTests
{
    private readonly IPersonRepository _personRepository;
    private readonly CreatePersonCommandHandler _sut;

    public CreatePersonCommandHandlerTests()
    {
        _personRepository = Substitute.For<IPersonRepository>();
        _sut = new CreatePersonCommandHandler(_personRepository, new NullLogger<CreatePersonCommandHandler>());
    }

    [Fact]
    public async Task Handle_Succeeds_When_Person_Created_Successfully()
    {
        var createDto = new CreatePersonDto("John", "Doe", "12345", "City", "Blau");
        var person = Person.Create(1, "John", "Doe", Address.Create("12345", "City").Value, Color.GetByName("Blau").Value).Value;

        _personRepository.AddPersonAsync(Arg.Any<Person>()).Returns(person);

        var command = new CreatePersonCommand(createDto);
        var result = await _sut.Handle(command, CancellationToken.None);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be("John");
            result.Value.LastName.Should().Be("Doe");
            result.Value.ZipCode.Should().Be("12345");
            result.Value.City.Should().Be("City");
        }
    }

    [Fact]
    public async Task Handle_Returns_Error_When_Invalid_Color_Provided()
    {
        var createDto = new CreatePersonDto("John", "Doe", "12345", "City", "InvalidColor");

        var command = new CreatePersonCommand(createDto);
        var result = await _sut.Handle(command, CancellationToken.None);

        using (new AssertionScope())
        {
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be(CommandErrors.Codes.CreatePersonFailedCode);
        }
    }

    [Fact]
    public async Task Handle_Returns_Error_When_Repository_Fails()
    {
        var createDto = new CreatePersonDto("John", "Doe", "12345", "City", "Blau");
        var error = new Error("TestError", "Test error message");

        _personRepository.AddPersonAsync(Arg.Any<Person>()).Returns(Result.Failure<Person, Error>(error));

        var command = new CreatePersonCommand(createDto);
        var result = await _sut.Handle(command, CancellationToken.None);

        using (new AssertionScope())
        {
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be(CommandErrors.Codes.CreatePersonFailedInternalCode);
        }
    }
}
