using FluentAssertions;
using Pereprodai.Catalog.Domain.Enums;
using Pereprodai.Catalog.Domain.ValueObjects;
using Pereprodai.Shared.Domain.Enums;

namespace Pereprodai.Catalog.UnitTests.Domain;

public class PriceTests
{
    [Fact]
    public void Equal_Prices_ShouldBeEqual()
    {
        var a = new Price(100, Currency.RUB);
        var b = new Price(100, Currency.RUB);

        a.Should().Be(b);
    }

    [Fact]
    public void Negative_Amount_ShouldThrow()
    {
        var act = () => new Price(-1, Currency.RUB);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Zero_Amount_ShouldBeValid()
    {
        var price = new Price(0, Currency.RUB);

        price.Amount.Should().Be(0);
    }

    [Fact]
    public void Different_Currency_ShouldNotBeEqual()
    {
        var a = new Price(100, Currency.RUB);
        var b = new Price(100, Currency.EUR);

        a.Should().NotBe(b);
    }

    [Fact]
    public void Different_Amount_ShouldNotBeEqual()
    {
        var a = new Price(100, Currency.RUB);
        var b = new Price(200, Currency.RUB);

        a.Should().NotBe(b);
    }

    [Fact]
    public void Equal_Prices_ShouldHaveSameHashCode()
    {
        var a = new Price(100, Currency.RUB);
        var b = new Price(100, Currency.RUB);

        a.GetHashCode().Should().Be(b.GetHashCode());
    }
}
