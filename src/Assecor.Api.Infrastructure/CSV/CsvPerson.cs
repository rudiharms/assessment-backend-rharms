using Assecor.Api.Domain.Common;
using Assecor.Api.Domain.Models;
using CSharpFunctionalExtensions;

namespace Assecor.Api.Infrastructure.Csv;

public class CsvPerson
{
    public string? LastName { get; set; }
    public string? FirstName { get; set; }
    public string? Address { get; set; }
    public int? ColorId { get; set; }

    public CsvPerson? HasNullOrEmptyData()
    {
        if (string.IsNullOrWhiteSpace(LastName) ||
            string.IsNullOrWhiteSpace(FirstName) ||
            string.IsNullOrWhiteSpace(Address) ||
            !ColorId.HasValue)
        {
            return this;
        }

        return null;
    }

    public Result<Person, Error> ToPerson(int id)
    {
        try
        {
            var firstName = FirstName?.Trim() ?? string.Empty;
            var lastName = LastName?.Trim() ?? string.Empty;

            var addressResult = ParseAddress(Address);

            if (addressResult.IsFailure)
            {
                return addressResult.Error;
            }

            var colorResult = ColorId.HasValue ? Color.GetById(ColorId.Value) : Color.None;

            if (colorResult.IsFailure)
            {
                return colorResult.Error;
            }

            var person = Person.Create(id, firstName, lastName, addressResult.Value, colorResult.Value);

            if (person.IsFailure)
            {
                return person.Error;
            }

            return person.Value;
        }
        catch (Exception e)
        {
            return Errors.CsvParsingFailed(e.Message);
        }
    }

    private static Result<Address, Error> ParseAddress(string? address)
    {
        var parts = address?.Split(' ', 2, StringSplitOptions.TrimEntries);

        return parts?.Length switch
        {
            1 => Domain.Models.Address.Create(parts[0], string.Empty),
            >= 2 => Domain.Models.Address.Create(parts[0], parts[1]),
            _ => Errors.AddressIsMissing
        };
    }
}
