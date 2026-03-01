using FluentAssertions;
using Pereprodai.Catalog.Domain.ValueObjects;

namespace Pereprodai.Catalog.UnitTests.Domain;

public class ContactInfoTests
{
    [Fact]
    public void Empty_Phone_ShouldThrow()
    {
        var act = () => new ContactInfo("");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Whitespace_Phone_ShouldThrow()
    {
        var act = () => new ContactInfo("   ");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Valid_Phone_WithoutEmail_ShouldCreate()
    {
        var contact = new ContactInfo("+79991234567");

        contact.Phone.Should().Be("+79991234567");
        contact.Email.Should().BeNull();
    }

    [Fact]
    public void Valid_Phone_WithEmail_ShouldCreate()
    {
        var contact = new ContactInfo("+79991234567", "test@gmail.com");

        contact.Phone.Should().Be("+79991234567");
        contact.Email.Should().Be("test@gmail.com");
    }

    [Fact]
    public void Equal_ContactInfos_ShouldBeEqual()
    {
        var a = new ContactInfo("+79991234567", "test@gmail.com");
        var b = new ContactInfo("+79991234567", "test@gmail.com");

        a.Should().Be(b);
    }

    [Fact]
    public void Equal_ContactInfos_WithNullEmail_ShouldBeEqual()
    {
        var a = new ContactInfo("+79991234567");
        var b = new ContactInfo("+79991234567");

        a.Should().Be(b);
    }

    [Fact]
    public void Different_Phone_ShouldNotBeEqual()
    {
        var a = new ContactInfo("+79991234567");
        var b = new ContactInfo("+79997654321");

        a.Should().NotBe(b);
    }

    [Fact]
    public void Different_Email_ShouldNotBeEqual()
    {
        var a = new ContactInfo("+79991234567", "one@gmail.com");
        var b = new ContactInfo("+79991234567", "two@gmail.com");

        a.Should().NotBe(b);
    }

    [Fact]
    public void NullEmail_Vs_WithEmail_ShouldNotBeEqual()
    {
        var a = new ContactInfo("+79991234567");
        var b = new ContactInfo("+79991234567", "test@gmail.com");

        a.Should().NotBe(b);
    }

    [Fact]
    public void Equal_ContactInfos_ShouldHaveSameHashCode()
    {
        var a = new ContactInfo("+79991234567", "test@gmail.com");
        var b = new ContactInfo("+79991234567", "test@gmail.com");

        a.GetHashCode().Should().Be(b.GetHashCode());
    }
}
