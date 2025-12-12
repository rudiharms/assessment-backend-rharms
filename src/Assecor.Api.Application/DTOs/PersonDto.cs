using System.Text.Json.Serialization;

namespace Assecor.Api.Application.DTOs;

public record PersonDto(
    int Id,
    string Name,
    [property: JsonPropertyName("lastname")]
    string LastName,
    [property: JsonPropertyName("zipcode")]
    string ZipCode,
    string City,
    string Color
);
