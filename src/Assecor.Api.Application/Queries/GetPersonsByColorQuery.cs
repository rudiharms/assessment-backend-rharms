using Assecor.Api.Application.DTOs;
using Assecor.Api.Domain.Common;
using CSharpFunctionalExtensions;
using MediatR;

namespace Assecor.Api.Application.Queries;

public record GetPersonsByColorQuery(string ColorName) : IRequest<Result<IEnumerable<PersonDto>, Error>>;
