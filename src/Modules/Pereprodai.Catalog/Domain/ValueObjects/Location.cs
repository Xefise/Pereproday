using Pereprodai.Shared.Domain;

namespace Pereprodai.Catalog.Domain.ValueObjects;

public class Location : ValueObject
{
    public string City { get; private set; } = null!;

    private Location() { } // EF Core

    public Location(string city)
    {
        if(string.IsNullOrWhiteSpace(city)) throw new ArgumentException("City cannot be empty", nameof(city));
        City = city;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return City;
    }
}
