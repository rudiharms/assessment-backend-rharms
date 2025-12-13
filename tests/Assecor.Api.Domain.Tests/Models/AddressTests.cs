using Assecor.Api.Domain.Models;
using FluentAssertions;
using FluentAssertions.Execution;

namespace Assecor.Api.Domain.Tests.Models;
//Note for Reviewer : Basic smoke tests, not 100% coverage

public class AddressTests
{
    [Fact]
    public void Create_Succeeds_When_Valid_Data_Provided()
    {
        var result = Address.Create("12345", "City");

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.ZipCode.Should().Be("12345");
            result.Value.City.Should().Be("City");
        }
    }

    [Fact]
    public void Create_Trims_And_Filters_ZipCode()
    {
        var result = Address.Create("  123-45  ", "City");

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.ZipCode.Should().Be("12345");
        }
    }

    [Fact]
    public void Create_Trims_And_Filters_City()
    {
        var result = Address.Create("12345", "  New123York  ");

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.City.Should().Be("NewYork");
        }
    }

    [Fact]
    public void Create_Returns_Error_When_ZipCode_Empty()
    {
        var result = Address.Create("", "City");

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Returns_Error_When_City_Empty()
    {
        var result = Address.Create("12345", "");

        result.IsFailure.Should().BeTrue();
    }
}
