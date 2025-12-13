using Assecor.Api.Application.DTOs;
using Assecor.Api.Domain.Common;
using CSharpFunctionalExtensions;
using MediatR;

namespace Assecor.Api.Application.Commands;

public record CreatePersonCommand(CreatePersonDto PersonDto) : IRequest<Result<PersonDto, Error>>;
