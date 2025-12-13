using Assecor.Api.Application.Commands;
using Assecor.Api.Application.DTOs;
using Assecor.Api.Application.Queries;
using Assecor.Api.Person.Common;
using Assecor.Api.Person.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Assecor.Api.Person.Controllers;

[ApiController]
[Route("/[controller]")]
public class PersonsController(ISender sender) : BaseController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreatePerson([FromBody] CreatePersonDto createPersonDto)
    {
        var result = await sender.Send(new CreatePersonCommand(createPersonDto));

        if (result.IsFailure)
        {
            if (result.Error.Code == CommandErrors.Codes.CreatePersonFailedPostPersistenceCode)
            {
                return StatusCode(
                    StatusCodes.Status201Created,
                    new { message = "Person created successfully, but could not be retrieved.", warning = result.Error.Message }
                );
            }

            return Problem(result.Error.ToProblemDetails());
        }

        return CreatedAtAction(nameof(GetPersonById), new { id = result.Value.Id }, result.Value);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPersons()
    {
        var result = await sender.Send(new GetPersonsQuery());

        if (result.IsFailure)
        {
            return Problem(result.Error.ToProblemDetails());
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPersonById(int id)
    {
        var result = await sender.Send(new GetPersonByIdQuery(id));

        if (result.IsFailure)
        {
            return Problem(result.Error.ToProblemDetails());
        }

        return Ok(result.Value);
    }

    /// <param name="colorName">Color name. Valid values: Blau, Grün, Violett, Rot, Gelb, Türkis, Weiß, None</param>
    [HttpGet("color/{colorName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPersonsByColor([FromRoute] string colorName)
    {
        var result = await sender.Send(new GetPersonsByColorQuery(colorName));

        if (result.IsFailure)
        {
            return Problem(result.Error.ToProblemDetails());
        }

        return Ok(result.Value);
    }
}
