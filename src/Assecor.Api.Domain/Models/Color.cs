using Assecor.Api.Domain.Common;
using CSharpFunctionalExtensions;

namespace Assecor.Api.Domain.Models;

public class Color
{
    private static readonly Color _blue = new(1, "blau");
    private static readonly Color _green = new(2, "grün");
    private static readonly Color _violet = new(3, "violett");
    private static readonly Color _red = new(4, "rot");
    private static readonly Color _yellow = new(5, "gelb");
    private static readonly Color _turquoise = new(6, "türkis");
    private static readonly Color _white = new(7, "weiß");

    private Color(int id, string name)
    {
        Id = id;
        Name = name;
    }

    private static IReadOnlyList<Color> All { get; } = new[] { _blue, _green, _violet, _red, _yellow, _turquoise, _white };

    public int Id { get; }
    public string Name { get; }

    public static Result<Color, Error> GetById(int id)
    {
        var color = All.FirstOrDefault(c => c.Id == id);

        return color ?? Result.Failure<Color, Error>(Errors.InvalidColor);
    }

    public static Result<Color, Error> GetByName(string name)
    {
        var color = All.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        return color ?? Result.Failure<Color, Error>(Errors.InvalidColor);
    }
}
