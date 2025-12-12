using Microsoft.AspNetCore.Mvc;

namespace Assecor.Api.Person.Common;

public class BaseController : ControllerBase
{
    [NonAction]
    protected static ObjectResult Problem(ProblemDetails problemDetails)
    {
        return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
    }
}
