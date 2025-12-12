using Assecor.Api.Application.Queries;
using Assecor.Api.Person.Common;
using Assecor.Api.Person.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Assecor.Api.Person.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonController(ISender sender) : BaseController
{
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

    /// <param name="colorName">Color name. Valid values: Blau, Gruen, Violett, Rot, Gelb, Tuerkis, Weiss, None</param>
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
