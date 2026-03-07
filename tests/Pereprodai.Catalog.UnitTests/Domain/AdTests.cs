using FluentAssertions;
using Pereprodai.Catalog.Domain.Entities;
using Pereprodai.Catalog.Domain.Enums;
using Pereprodai.Catalog.Domain.ValueObjects;
using Pereprodai.Shared.Domain.Enums;
using Pereprodai.Shared.Domain.Events.Catalog;

namespace Pereprodai.Catalog.UnitTests.Domain;

public class AdTests
{
    private static Ad CreateDraftAd(Guid? userId = null)
    {
        var ad = Ad.Create(userId ?? Guid.NewGuid(), "Title", "Description", "Category",
            new Price(1923, Currency.RUB),
            new Location("Krd"),
            new ContactInfo("1234567890", null));

        return ad;
    }

    [Fact]
    public void Create_ShouldReturnDraftAd_WithCreatedEvent()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var ad = CreateDraftAd(userId);

        // Assert
        ad.Status.Should().Be(AdStatus.Draft);
        ad.Id.Should().NotBeEmpty();

        var domainEvent = ad.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<AdCreatedEvent>().Subject;

        domainEvent.AdId.Should().Be(ad.Id);
        domainEvent.UserId.Should().Be(userId);
    }

    [Fact]
    public void SubmitForModeration_FromDraft_ShouldChangeStatus()
    {
        // Arrange
        var ad = CreateDraftAd();

        // Act
        ad.SubmitForModeration();

        // Assert
        ad.Status.Should().Be(AdStatus.OnModeration);

        var domainEvent = ad.DomainEvents.Should().Contain(e => e is AdSubmittedForModerationEvent)
            .Which.Should().BeOfType<AdSubmittedForModerationEvent>().Subject;

        domainEvent.Snapshot.AdId.Should().Be(ad.Id);
    }

    [Fact]
    public void Publish_FromOnModeration_ShouldChangeStatus()
    {
        // Arrange
        var ad = CreateDraftAd();
        ad.SubmitForModeration();

        // Act
        ad.Publish();

        // Assert
        ad.Status.Should().Be(AdStatus.Published);

        var domainEvent = ad.DomainEvents.Should().Contain(e => e is AdPublishedEvent)
            .Which.Should().BeOfType<AdPublishedEvent>().Subject;

        domainEvent.AdSnapshot.AdId.Should().Be(ad.Id);
    }
    [Fact]
    public void Publish_FromDraft_ShouldThrow()
    {
        // Arrange
        var ad = CreateDraftAd();

        // Act & Assert
        var act = () => ad.Publish();
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Reject_FromOnModeration_ShouldChangeStatus()
    {
        // Arrange
        var ad = CreateDraftAd();
        ad.SubmitForModeration();

        // Act
        ad.Reject("da patamy 4ta");

        // Assert
        ad.Status.Should().Be(AdStatus.Rejected);

        var domainEvent = ad.DomainEvents.Should().Contain(e => e is AdRejectedEvent)
            .Which.Should().BeOfType<AdRejectedEvent>().Subject;

        domainEvent.AdId.Should().Be(ad.Id);
        domainEvent.Reason.Should().Be("da patamy 4ta");
    }
    [Fact]
    public void Reject_FromDraft_ShouldThrow()
    {
        // Arrange
        var ad = CreateDraftAd();

        // Act & Assert
        var act = () => ad.Reject("da patamy 4ta");
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ReturnToDraft_FromRejected_ShouldChangeStatus()
    {
        // Arrange
        var ad = CreateDraftAd();
        ad.SubmitForModeration();
        ad.Reject("da patamy 4ta");

        // Act
        ad.ReturnToDraft();

        // Assert
        ad.Status.Should().Be(AdStatus.Draft);
    }
    [Fact]
    public void ReturnToDraft_FromDraft_ShouldThrow()
    {
        // Arrange
        var ad = CreateDraftAd();

        // Act & Assert
        var act = () => ad.ReturnToDraft();
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Archive_FromOnDraft_ShouldChangeStatus()
    {
        // Arrange
        var ad = CreateDraftAd();

        // Act
        ad.Archive();

        // Assert
        ad.Status.Should().Be(AdStatus.Archived);

        var domainEvent = ad.DomainEvents.Should().Contain(e => e is AdArchivedEvent)
            .Which.Should().BeOfType<AdArchivedEvent>().Subject;

        domainEvent.AdId.Should().Be(ad.Id);
    }
    [Fact]
    public void Archive_FromArchive_ShouldThrow()
    {
        // Arrange
        var ad = CreateDraftAd();
        ad.Archive();

        // Act & Assert
        var act = () => ad.Archive();
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Update_FromOnPublished_ShouldUpdateAndChangeStatus()
    {
        // Arrange
        var ad = CreateDraftAd();
        ad.SubmitForModeration();
        ad.Publish();

        // Act
        ad.Update("T2", "D2", "C2",
            new Price(1, Currency.EUR),
            new Location("Moscow"), new ContactInfo("1234567891", "email@email.com"));

        // Assert
        ad.Status.Should().Be(AdStatus.OnModeration);

        ad.Title.Should().Be("T2");
        ad.Description.Should().Be("D2");
        ad.Category.Should().Be("C2");
        ad.Price.Amount.Should().Be(1);
        ad.Price.Currency.Should().Be(Currency.EUR);
        ad.Location.City.Should().Be("Moscow");
        ad.ContactInfo.Phone.Should().Be("1234567891");
        ad.ContactInfo.Email.Should().Be("email@email.com");

        ad.DomainEvents.Should().Contain(e => e is AdUpdatedEvent);
        ad.DomainEvents.Should().Contain(e => e is AdSubmittedForModerationEvent);
    }
    [Fact]
    public void Update_FromArchive_ShouldThrow()
    {
        // Arrange
        var ad = CreateDraftAd();
        ad.Archive();

        // Act & Assert
        var act = () => ad.Update("Title", "Description", "Category",
            new Price(1923, Currency.RUB),
            new Location("Moscow"), new ContactInfo("1234567890", "email"));
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void EnsureOwner_WithCorrectUserId_ShouldPass()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ad = CreateDraftAd(userId);

        // Act
        ad.EnsureOwner(userId);
    }
    [Fact]
    public void EnsureOwner_WithWrongUserId_ShouldThrow()
    {
        // Arrange
        var ad = CreateDraftAd();

        // Act & Assert
        var act = () => ad.EnsureOwner(Guid.NewGuid());
        act.Should().Throw<UnauthorizedAccessException>();
    }
}
