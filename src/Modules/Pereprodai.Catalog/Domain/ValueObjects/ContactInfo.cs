using Pereprodai.Shared.Domain;

namespace Pereprodai.Catalog.Domain.ValueObjects;

public class ContactInfo : ValueObject
{
    public string Phone { get; private set; } = null!;
    public string? Email { get; private set; }

    private ContactInfo() { } // EF Core

    public ContactInfo(string phone, string? email = null)
    {
        if(string.IsNullOrWhiteSpace(phone)) throw new ArgumentException("Phone cannot be empty", nameof(phone));
        Phone = phone;
        Email = email;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Phone;
        yield return Email;
    }
}
