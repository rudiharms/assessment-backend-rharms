using Assecor.Api.Domain.Common;
using CSharpFunctionalExtensions;

namespace Assecor.Api.Domain.Models;

public class Address(string zipCode, string city)
{
    public string ZipCode { get; private set; } = zipCode;
    public string City { get; private set; } = city;

    public static Result<Address, Error> Create(string zipCode, string city)
    {
        zipCode = zipCode.Trim();
        zipCode = new string([.. zipCode.Where(char.IsAsciiDigit)]);

        if (string.IsNullOrWhiteSpace(zipCode))
        {
            return Errors.InvalidAddressZipCode;
        }

        city = city.Trim();
        city = new string([.. city.Where(char.IsLetter)]);

        if (string.IsNullOrWhiteSpace(city))
        {
            return Errors.InvalidAddressCity;
        }

        return new Address(zipCode, city);
    }
}
