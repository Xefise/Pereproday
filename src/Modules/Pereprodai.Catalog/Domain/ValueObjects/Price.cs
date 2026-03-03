using Pereprodai.Catalog.Domain.Enums;
using Pereprodai.Shared.Domain;
using Pereprodai.Shared.Domain.Enums;

namespace Pereprodai.Catalog.Domain.ValueObjects;

public class Price : ValueObject
{
    public decimal Amount { get; private set; }
    public Currency Currency { get; private set; }

    private Price() { } // EF Core

    public Price(decimal amount, Currency currency)
    {
        if(amount < 0) throw new ArgumentException("Amount cannot be negative", nameof(amount));
        Amount = amount;
        Currency = currency;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
