using Assecor.Api.Domain.Common;
using Assecor.Api.Domain.Enums;
using CSharpFunctionalExtensions;

namespace Assecor.Api.Domain.Models;

public class Color
{
    private static readonly Color _blue = new(ColorName.Blau);
    private static readonly Color _green = new(ColorName.Grün);
    private static readonly Color _violet = new(ColorName.Violett);
    private static readonly Color _red = new(ColorName.Rot);
    private static readonly Color _yellow = new(ColorName.Gelb);
    private static readonly Color _turquoise = new(ColorName.Türkis);
    private static readonly Color _white = new(ColorName.Weiß);
    public static readonly Color None = new(ColorName.None);

    private Color(ColorName colorName)
    {
        ColorName = colorName;
        Id = (int) colorName;
    }

    private static IReadOnlyList<Color> All { get; } = new[] { _blue, _green, _violet, _red, _yellow, _turquoise, _white, None };

    public int Id { get; }
    public ColorName ColorName { get; }

    public static Result<Color, Error> GetById(int id)
    {
        var color = All.FirstOrDefault(c => c.Id == id);

        return color ?? Result.Failure<Color, Error>(Errors.InvalidColor);
    }

    public static Result<Color, Error> GetByName(string name)
    {
        var color = All.FirstOrDefault(c => c.ColorName.ToString().Equals(name, StringComparison.OrdinalIgnoreCase));

        return color ?? Result.Failure<Color, Error>(Errors.InvalidColor);
    }
}
