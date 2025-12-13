using Assecor.Api.Domain.Enums;
using Assecor.Api.Domain.Models;
using FluentAssertions;
using FluentAssertions.Execution;

namespace Assecor.Api.Domain.Tests.Models;
//Note for Reviewer : Basic smoke tests, not 100% coverage

public class ColorTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    public void GetById_Succeeds_When_Valid_Id_Provided(int id)
    {
        var result = Color.GetById(id);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be(id);
        }
    }

    [Fact]
    public void GetById_Returns_Error_When_Invalid_Id_Provided()
    {
        var result = Color.GetById(999);

        result.IsFailure.Should().BeTrue();
    }

    [Theory]
    [InlineData("Blau", ColorName.Blau)]
    [InlineData("Grün", ColorName.Grün)]
    [InlineData("Rot", ColorName.Rot)]
    public void GetByName_Succeeds_When_Valid_Name_Provided(string name, ColorName expectedColorName)
    {
        var result = Color.GetByName(name);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.ColorName.Should().Be(expectedColorName);
        }
    }

    [Theory]
    [InlineData("blau")]
    [InlineData("BLAU")]
    [InlineData("bLaU")]
    public void GetByName_Is_Case_Insensitive(string name)
    {
        var result = Color.GetByName(name);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.ColorName.Should().Be(ColorName.Blau);
        }
    }

    [Fact]
    public void GetByName_Returns_Error_When_Invalid_Name_Provided()
    {
        var result = Color.GetByName("InvalidColor");

        result.IsFailure.Should().BeTrue();
    }
}
