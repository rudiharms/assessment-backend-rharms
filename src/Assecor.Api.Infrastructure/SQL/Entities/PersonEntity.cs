using Assecor.Api.Domain.Common;
using Assecor.Api.Domain.Enums;
using Assecor.Api.Domain.Models;
using CSharpFunctionalExtensions;

namespace Assecor.Api.Infrastructure.Sql.Entities;

public class PersonEntity
{
    public const int MaxFirstNameLength = 200;
    public const int MaxLastNameLength = 200;
    public const int MaxZipCodeLength = 20;
    public const int MaxCityLength = 200;

#pragma warning disable CS8618 // Default constructor as required by EF Core
    private PersonEntity() { }
#pragma warning restore CS8618

    private PersonEntity(int id, string firstName, string lastName, string zipCode, string city, int colorId)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        ZipCode = zipCode;
        City = city;
        ColorId = colorId;
    }

    public int Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string ZipCode { get; init; }
    public string City { get; init; }
    public int ColorId { get; init; } = (int) ColorName.None;

    public static Result<PersonEntity, Error> Create(
        string firstName,
        string lastName,
        string zipCode,
        string city,
        int colorId,
        int id = 0
    )
    {
        firstName = firstName.Trim();
        lastName = lastName.Trim();
        zipCode = zipCode.Trim();
        city = city.Trim();

        if (firstName.Length > MaxFirstNameLength)
        {
            return Errors.PersonEntityValidationFailed(nameof(FirstName), MaxFirstNameLength);
        }

        if (lastName.Length > MaxLastNameLength)
        {
            return Errors.PersonEntityValidationFailed(nameof(LastName), MaxLastNameLength);
        }

        if (zipCode.Length > MaxZipCodeLength)
        {
            return Errors.PersonEntityValidationFailed(nameof(ZipCode), MaxZipCodeLength);
        }

        if (city.Length > MaxCityLength)
        {
            return Errors.PersonEntityValidationFailed(nameof(City), MaxCityLength);
        }

        return new PersonEntity(id, firstName, lastName, zipCode, city, colorId);
    }

    public static Result<PersonEntity, Error> FromPerson(Person person)
    {
        return Create(
            person.FirstName,
            person.LastName,
            person.Address.ZipCode,
            person.Address.City,
            (int) person.Color.ColorName,
            person.Id
        );
    }

    public Result<Person, Error> ToPerson()
    {
        try
        {
            var firstName = FirstName.Trim();
            var lastName = LastName.Trim();

            var addressResult = Address.Create(ZipCode, City);

            if (addressResult.IsFailure)
            {
                return addressResult.Error;
            }

            var colorResult = Color.GetById(ColorId);

            if (colorResult.IsFailure)
            {
                return colorResult.Error;
            }

            var person = Person.Create(Id, firstName, lastName, addressResult.Value, colorResult.Value);

            if (person.IsFailure)
            {
                return person.Error;
            }

            return person.Value;
        }
        catch (Exception e)
        {
            return Errors.PersonEntityParsingFailed(e.Message);
        }
    }
}
