using Pereprodai.Catalog.Domain.Enums;
using Pereprodai.Catalog.Domain.Events;
using Pereprodai.Catalog.Domain.ValueObjects;
using Pereprodai.Shared.Domain;

namespace Pereprodai.Catalog.Domain.Entities;

public class Ad : AggregateRoot
{
    public Guid UserId { get; private set; }
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Category { get; private set; } = null!;
    public Price Price { get; private set; } = null!;
    public Location Location { get; private set; } = null!;
    public ContactInfo ContactInfo { get; private set; } = null!;
    public AdStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Ad() { } // EF Core

    public static Ad Create(
        Guid userId,
        string title,
        string description,
        string category,
        Price price,
        Location location,
        ContactInfo contactInfo)
    {
        var ad = new Ad()
        {
            Id = Guid.NewGuid(),
            Status = AdStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Title = title,
            Description = description,
            Category = category,
            Price = price,
            Location = location,
            ContactInfo = contactInfo,
            UserId = userId,
        };

        ad.RaiseDomainEvent(new AdCreatedEvent(ad.Id, userId));

        return ad;
    }

    public void Update(
        string title,
        string description,
        string category,
        Price price,
        Location location,
        ContactInfo contactInfo)
    {
        var allowedStatuses = new[] { AdStatus.Draft, AdStatus.OnModeration, AdStatus.Published };
        if(!allowedStatuses.Contains(Status))
            throw new InvalidOperationException("Can't update an ad that is not in Draft, OnModeration or Published status.");

        Title = title;
        Description = description;
        Category = category;
        Price = price;
        Location = location;
        ContactInfo = contactInfo;
        UpdatedAt = DateTime.UtcNow;

        if(Status is AdStatus.Published or AdStatus.OnModeration) Status = AdStatus.OnModeration;

        RaiseDomainEvent(new AdUpdatedEvent(Id));
    }

    public void SubmitForModeration()
    {
        if(Status != AdStatus.Draft)
            throw new InvalidOperationException("Can't submit an ad that is not in Draft status.");

        Status = AdStatus.OnModeration;
        RaiseDomainEvent(new AdSubmittedForModerationEvent(Id));
    }

    public void Publish()
    {
        if(Status != AdStatus.OnModeration)
            throw new InvalidOperationException("Can't publish an ad that is not in OnModeration status.");

        Status = AdStatus.Published;
        RaiseDomainEvent(new AdPublishedEvent(Id));
    }

    public void Reject(string reason)
    {
        if(Status != AdStatus.OnModeration)
            throw new InvalidOperationException("Can't reject an ad that is not in OnModeration status.");

        Status = AdStatus.Rejected;
        RaiseDomainEvent(new AdRejectedEvent(Id, reason));
    }

    public void ReturnToDraft()
    {
        if(Status != AdStatus.Rejected)
            throw new InvalidOperationException("Can't return an ad that is not in Rejected status.");

        Status = AdStatus.Draft;
    }

    public void Archive()
    {
        if(Status == AdStatus.Archived)
            throw new InvalidOperationException("Can't archive an ad that is already archived.");

        Status = AdStatus.Archived;
        RaiseDomainEvent(new AdArchivedEvent(Id));
    }

    public void EnsureOwner(Guid userId)
    {
        if(UserId != userId) throw new UnauthorizedAccessException("You are not the owner of this ad.");
    }
}
