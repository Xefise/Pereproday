using Pereprodai.Moderation.Domain.Entities;
using Pereprodai.Shared.Domain.Enums;
using Pereprodai.Shared.Domain.Events.Catalog.Snapshots;

namespace Pereprodai.Moderation.Infrastructure.ReadModels;

public class AdReadModel
{
    public Guid AdId { get; private set; }
    public Guid UserId { get; private set; }
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Category { get; private set; } = null!;
    public decimal PriceAmount { get; private set; }
    public Currency PriceCurrency { get; private set; }
    public string LocationCity { get; private set; } = null!;
    public string ContactInfoPhone { get; private set; } = null!;
    public string? ContactInfoEmail { get; private set; }

    public List<ModerationTask> ModerationTasks { get; private set; } = [];

    private AdReadModel() { }

    public AdReadModel(AdSnapshot snapshot)
    {
        AdId = snapshot.AdId;
        UserId = snapshot.UserId;
        Title = snapshot.Title;
        Description = snapshot.Description;
        Category = snapshot.Category;
        PriceAmount = snapshot.PriceAmount;
        PriceCurrency = snapshot.PriceCurrency;
        LocationCity = snapshot.LocationCity;
        ContactInfoPhone = snapshot.ContactInfoPhone;
        ContactInfoEmail = snapshot.ContactInfoEmail;
    }

    public void Update(AdSnapshot snapshot)
    {
        Title = snapshot.Title;
        Description = snapshot.Description;
        Category = snapshot.Category;
        PriceAmount = snapshot.PriceAmount;
        PriceCurrency = snapshot.PriceCurrency;
        LocationCity = snapshot.LocationCity;
        ContactInfoPhone = snapshot.ContactInfoPhone;
        ContactInfoEmail = snapshot.ContactInfoEmail;
    }
}