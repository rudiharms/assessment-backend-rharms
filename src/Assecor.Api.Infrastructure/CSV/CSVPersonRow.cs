using Assecor.Api.Application.DTOs;
using Assecor.Api.Domain.Common;
using CSharpFunctionalExtensions;

namespace Assecor.Api.Infrastructure.CSV;

public class CsvPersonRow
{
    public string? LastName { get; set; }
    public string? FirstName { get; set; }
    public string? Address { get; set; }
    public int? ColorId { get; set; }

    public Result<PersonDto, Error> ToPersonDto()
    {
        try
        {
            var firstName = FirstName?.Trim() ?? string.Empty;
            var lastName = LastName?.Trim() ?? string.Empty;

            var addressDto = ParseAddress(Address);
            var colorDto = new ColorDto(ColorId);

            return new PersonDto(firstName, lastName, addressDto, colorDto);
        }
        catch (Exception e)
        {
            return Errors.CsvParsingFailed(e.Message);
        }
    }

    private static AddressDto ParseAddress(string? address)
    {
        var parts = address?.Split(' ', 2, StringSplitOptions.TrimEntries);

        return parts?.Length switch
        {
            1 => new AddressDto(parts[0], null),
            >= 2 => new AddressDto(parts[0], parts[1]),
            _ => new AddressDto(null, null)
        };
    }
}
