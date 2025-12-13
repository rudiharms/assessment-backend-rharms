using Assecor.Api.Domain.Common;
using Assecor.Api.Domain.Models;
using CSharpFunctionalExtensions;

namespace Assecor.Api.Infrastructure.Csv;

public class CsvPerson
{
    private CsvPerson(string lastName, string firstName, string address, int colorId)
    {
        LastName = lastName;
        FirstName = firstName;
        Address = address;
        ColorId = colorId;
    }

    public string LastName { get; }
    public string FirstName { get; }
    public string Address { get; }
    public int ColorId { get; }

    public static Result<CsvPerson, Error> Create(string? lastName, string? firstName, string? address, int? colorId)
    {
        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Errors.CsvPersonCreationFailed("LastName cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Errors.CsvPersonCreationFailed("FirstName cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(address))
        {
            return Errors.CsvPersonCreationFailed("Address cannot be empty");
        }

        if (!colorId.HasValue)
        {
            return Errors.CsvPersonCreationFailed("ColorId cannot be null");
        }

        return new CsvPerson(lastName.Trim(), firstName.Trim(), address.Trim(), colorId.Value);
    }

    public static Result<CsvPerson, Error> FromPerson(Person person)
    {
        var address = $"{person.Address.ZipCode} {person.Address.City}";
        var colorId = (int) person.Color.ColorName;

        return Create(person.LastName, person.FirstName, address, colorId);
    }

    public Result<Person, Error> ToPerson(int id)
    {
        try
        {
            var addressResult = ParseAddress(Address);

            if (addressResult.IsFailure)
            {
                return addressResult.Error;
            }

            var colorResult = Color.GetById(ColorId);

            if (colorResult.IsFailure)
            {
                return colorResult.Error;
            }

            var person = Person.Create(id, FirstName, LastName, addressResult.Value, colorResult.Value);

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

    private static Result<Address, Error> ParseAddress(string address)
    {
        var parts = address.Split(' ', 2, StringSplitOptions.TrimEntries);

        return parts.Length switch
        {
            1 => Domain.Models.Address.Create(parts[0], string.Empty),
            >= 2 => Domain.Models.Address.Create(parts[0], parts[1]),
            _ => Errors.AddressIsMissing
        };
    }
}
