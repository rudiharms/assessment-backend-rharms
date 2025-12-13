using Assecor.Api.Application.Commands;
using Assecor.Api.Application.DTOs;
using Assecor.Api.Application.Queries;
using Assecor.Api.Domain.Common;
using Assecor.Api.Domain.Enums;
using Assecor.Api.Person.Controllers;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Assecor.Api.Person.Tests.Controllers;

public class PersonsControllerTests
{
    private readonly IFixture _fixture;
    private readonly ISender _sender;
    private readonly PersonsController _sut;

    public PersonsControllerTests()
    {
        _sender = Substitute.For<ISender>();
        _sut = new PersonsController(_sender);
        _fixture = new Fixture();
    }

    #region CreatePerson Tests

    [Fact]
    public async Task CreatePerson_Succeeds_When_Person_Created_Successfully()
    {
        var createDto = _fixture.Create<CreatePersonDto>();
        var personDto = _fixture.Create<PersonDto>();

        _sender.Send(Arg.Any<CreatePersonCommand>(), Arg.Any<CancellationToken>()).Returns(personDto);

        var result = await _sut.CreatePerson(createDto);

        using (new AssertionScope())
        {
            await _sender.Received(1).Send(Arg.Is<CreatePersonCommand>(cmd => cmd.PersonDto == createDto), Arg.Any<CancellationToken>());

            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult!.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(personDto.Id);
            createdResult.Value.Should().BeEquivalentTo(personDto);
            createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
        }
    }

    [Fact]
    public async Task CreatePerson_Succeeds_With_Warning_When_PostPersistence_Error_Occurs()
    {
        var createDto = _fixture.Create<CreatePersonDto>();
        var error = new Error(CommandErrors.Codes.CreatePersonFailedPostPersistenceCode, "Person was created, but could not be retrieved");

        _sender.Send(Arg.Any<CreatePersonCommand>(), Arg.Any<CancellationToken>()).Returns(error);

        var result = await _sut.CreatePerson(createDto);

        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(StatusCodes.Status201Created);
            var value = objectResult.Value;
            value.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task CreatePerson_Returns_BadRequest_When_CreatePersonFailed_Error_Occurs()
    {
        var createDto = _fixture.Create<CreatePersonDto>();
        var error = new Error(CommandErrors.Codes.CreatePersonFailedCode, "Validation failed");

        _sender.Send(Arg.Any<CreatePersonCommand>(), Arg.Any<CancellationToken>()).Returns(error);

        var actionResult = await _sut.CreatePerson(createDto);

        using (new AssertionScope())
        {
            actionResult.Should().BeOfType<ObjectResult>();
            var objectResult = actionResult as ObjectResult;
            objectResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            objectResult!.Value.Should().BeOfType<ProblemDetails>();
            var problemDetails = objectResult.Value as ProblemDetails;
            problemDetails!.Type.Should().Be(error.Code);
        }
    }

    [Fact]
    public async Task CreatePerson_Returns_InternalServerError_When_CreatePersonFailedInternal_Error_Occurs()
    {
        var createDto = _fixture.Create<CreatePersonDto>();
        var error = new Error(CommandErrors.Codes.CreatePersonFailedInternalCode, "Internal error");

        _sender.Send(Arg.Any<CreatePersonCommand>(), Arg.Any<CancellationToken>()).Returns(error);

        var result = await _sut.CreatePerson(createDto);

        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            objectResult!.Value.Should().BeOfType<ProblemDetails>();
            var problemDetails = objectResult.Value as ProblemDetails;
            problemDetails!.Type.Should().Be(error.Code);
        }
    }

    #endregion

    #region GetPersons Tests

    [Fact]
    public async Task GetPersons_Succeeds_When_Persons_Exist()
    {
        var persons = _fixture.CreateMany<PersonDto>(5).ToList();

        _sender.Send(Arg.Any<GetPersonsQuery>(), Arg.Any<CancellationToken>()).Returns(persons);

        var result = await _sut.GetPersons();

        using (new AssertionScope())
        {
            await _sender.Received(1).Send(Arg.Any<GetPersonsQuery>(), Arg.Any<CancellationToken>());

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(persons);
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        }
    }

    [Fact]
    public async Task GetPersons_Succeeds_When_No_Persons_Exist()
    {
        var persons = Enumerable.Empty<PersonDto>().ToList();

        _sender.Send(Arg.Any<GetPersonsQuery>(), Arg.Any<CancellationToken>()).Returns(persons);

        var result = await _sut.GetPersons();

        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(persons);
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            (okResult.Value as IEnumerable<PersonDto>).Should().BeEmpty();
        }
    }

    [Fact]
    public async Task GetPersons_Returns_Problem_When_Query_Fails()
    {
        var error = new Error(QueryErrors.Codes.PersonsQueryFailedCode, "Query failed");

        _sender.Send(Arg.Any<GetPersonsQuery>(), Arg.Any<CancellationToken>()).Returns(error);

        var result = await _sut.GetPersons();

        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult!.Value.Should().BeOfType<ProblemDetails>();
            var problemDetails = objectResult.Value as ProblemDetails;
            problemDetails!.Type.Should().Be(QueryErrors.Codes.PersonsQueryFailedCode);
            problemDetails.Status.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }

    #endregion

    #region GetPersonById Tests

    [Fact]
    public async Task GetPersonById_Succeeds_When_Persons_Exist()
    {
        var personDto = _fixture.Create<PersonDto>();

        _sender.Send(Arg.Any<GetPersonByIdQuery>(), Arg.Any<CancellationToken>()).Returns(personDto);

        var result = await _sut.GetPersonById(personDto.Id);

        using (new AssertionScope())
        {
            await _sender.Received(1).Send(Arg.Is<GetPersonByIdQuery>(q => q.Id == personDto.Id), Arg.Any<CancellationToken>());

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(personDto);
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        }
    }

    [Fact]
    public async Task GetPersonById_Returns_NotFound_When_Person_Does_Not_Exist()
    {
        var personId = _fixture.Create<int>();
        var error = new Error(QueryErrors.Codes.PersonNotFoundCode, $"Person not found for Id: {personId}");

        _sender.Send(Arg.Any<GetPersonByIdQuery>(), Arg.Any<CancellationToken>()).Returns(error);

        var result = await _sut.GetPersonById(personId);

        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult!.Value.Should().BeOfType<ProblemDetails>();
            var problemDetails = objectResult.Value as ProblemDetails;
            problemDetails!.Status.Should().Be(StatusCodes.Status404NotFound);
            problemDetails.Type.Should().Be(QueryErrors.Codes.PersonNotFoundCode);
        }
    }

    [Fact]
    public async Task GetPersonById_Returns_InternalServerError_When_Unknown_Error_Occurs()
    {
        var personId = _fixture.Create<int>();
        var error = new Error(QueryErrors.Codes.UnknownErrorCode, "Unknown error occurred");

        _sender.Send(Arg.Any<GetPersonByIdQuery>(), Arg.Any<CancellationToken>()).Returns(error);

        var result = await _sut.GetPersonById(personId);

        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }

    #endregion

    #region GetPersonsByColor Tests

    [Theory]
    [InlineData(nameof(ColorName.Blau))]
    [InlineData(nameof(ColorName.Grün))]
    [InlineData(nameof(ColorName.Violett))]
    [InlineData(nameof(ColorName.Rot))]
    [InlineData(nameof(ColorName.Gelb))]
    [InlineData(nameof(ColorName.Türkis))]
    [InlineData(nameof(ColorName.Weiß))]
    public async Task GetPersonsByColor_Succeeds_When_Valid_Color_Provided(string colorName)
    {
        var persons = _fixture.CreateMany<PersonDto>(3).ToList();

        _sender.Send(Arg.Any<GetPersonsByColorQuery>(), Arg.Any<CancellationToken>()).Returns(persons);

        var result = await _sut.GetPersonsByColor(colorName);

        using (new AssertionScope())
        {
            await _sender.Received(1).Send(Arg.Is<GetPersonsByColorQuery>(q => q.ColorName == colorName), Arg.Any<CancellationToken>());

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(persons);
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        }
    }

    [Fact]
    public async Task GetPersonsByColor_Succeeds_When_No_Persons_Match_Color()
    {
        var colorName = nameof(ColorName.Blau);
        var persons = Enumerable.Empty<PersonDto>().ToList();

        _sender.Send(Arg.Any<GetPersonsByColorQuery>(), Arg.Any<CancellationToken>()).Returns(persons);

        var result = await _sut.GetPersonsByColor(colorName);

        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            (okResult!.Value as IEnumerable<PersonDto>).Should().BeEmpty();
        }
    }

    [Theory]
    [InlineData("InvalidColor")]
    [InlineData("")]
    [InlineData("123")]
    public async Task GetPersonsByColor_Returns_BadRequest_When_Invalid_Color_Provided(string invalidColor)
    {
        var error = new Error(QueryErrors.Codes.InvalidColorCode, $"Invalid color: {invalidColor}");

        _sender.Send(Arg.Any<GetPersonsByColorQuery>(), Arg.Any<CancellationToken>()).Returns(error);

        var result = await _sut.GetPersonsByColor(invalidColor);

        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult!.Value.Should().BeOfType<ProblemDetails>();
            var problemDetails = objectResult.Value as ProblemDetails;
            problemDetails!.Status.Should().Be(StatusCodes.Status400BadRequest);
            problemDetails.Type.Should().Be(QueryErrors.Codes.InvalidColorCode);
        }
    }

    [Fact]
    public async Task GetPersonsByColor_Returns_InternalServerError_When_ColorQueryFailed_Error_Occurs()
    {
        var colorName = nameof(ColorName.Blau);
        var error = new Error(QueryErrors.Codes.ColorQueryFailedCode, "Query failed");

        _sender.Send(Arg.Any<GetPersonsByColorQuery>(), Arg.Any<CancellationToken>()).Returns(error);

        var result = await _sut.GetPersonsByColor(colorName);

        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            var problemDetails = objectResult.Value as ProblemDetails;
            problemDetails!.Type.Should().Be(QueryErrors.Codes.ColorQueryFailedCode);
        }
    }

    [Theory]
    [InlineData("blau")]
    [InlineData("BLAU")]
    [InlineData("bLaU")]
    public async Task GetPersonsByColor_Handles_Different_Case_Variations(string colorName)
    {
        var persons = _fixture.CreateMany<PersonDto>(2).ToList();

        _sender.Send(Arg.Any<GetPersonsByColorQuery>(), Arg.Any<CancellationToken>()).Returns(persons);

        await _sut.GetPersonsByColor(colorName);

        await _sender.Received(1).Send(Arg.Is<GetPersonsByColorQuery>(q => q.ColorName == colorName), Arg.Any<CancellationToken>());
    }

    #endregion
}
