namespace Assecor.Api.Application.DTOs;

public record PersonDto(string FirstName, string LastName, AddressDto Address, ColorDto Color);
