namespace Assecor.Api.Domain.Models;

public class Person(int id, string firstName, string lastName, Address address, Color color)
{
    public int Id { get; private set; } = id;
    public string FirstName { get; private set; } = firstName;
    public string LastName { get; private set; } = lastName;
    public Address Address { get; private set; } = address;
    public Color Color { get; private set; } = color;
}
