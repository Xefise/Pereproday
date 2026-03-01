using FluentAssertions;
using Pereprodai.Catalog.Domain.ValueObjects;

namespace Pereprodai.Catalog.UnitTests.Domain;

public class LocationTests
{
    [Fact]
    public void Empty_City_ShouldThrow()
    {
        var act = () => new Location("");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Whitespace_City_ShouldThrow()
    {
        var act = () => new Location("   ");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Valid_City_ShouldCreate()
    {
        var location = new Location("Краснодар");

        location.City.Should().Be("Краснодар");
    }

    [Fact]
    public void Equal_Locations_ShouldBeEqual()
    {
        var a = new Location("Москва");
        var b = new Location("Москва");

        a.Should().Be(b);
    }

    [Fact]
    public void Different_Locations_ShouldNotBeEqual()
    {
        var a = new Location("Москва");
        var b = new Location("Краснодар");

        a.Should().NotBe(b);
    }

    [Fact]
    public void Equal_Locations_ShouldHaveSameHashCode()
    {
        var a = new Location("Москва");
        var b = new Location("Москва");

        a.GetHashCode().Should().Be(b.GetHashCode());
    }
}
