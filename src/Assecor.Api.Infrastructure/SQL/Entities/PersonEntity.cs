using Assecor.Api.Domain.Common;
using Assecor.Api.Domain.Enums;
using Assecor.Api.Domain.Models;
using CSharpFunctionalExtensions;

namespace Assecor.Api.Infrastructure.Sql.Entities;

public class PersonEntity
{
    public int Id { get; private set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string ZipCode { get; set; }
    public required string City { get; set; }
    public int ColorId { get; set; } = (int) ColorName.None;

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
