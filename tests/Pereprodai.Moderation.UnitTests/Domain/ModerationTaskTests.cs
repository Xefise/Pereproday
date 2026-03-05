using FluentAssertions;
using Pereprodai.Moderation.Domain.Entities;
using Pereprodai.Moderation.Domain.Enums;
using Pereprodai.Shared.Domain.Events.Moderation;

namespace Pereprodai.Moderation.UnitTests.Domain;

public class ModerationTaskTests
{
    private static ModerationTask CreateModerationTask(Guid adId)
    {
        return ModerationTask.Create(adId);
    }

    [Fact]
    public void Create_ShouldCreateModerationTask()
    {
        var moderationTask = CreateModerationTask(Guid.NewGuid());

        moderationTask.Status.Should().Be(ModerationStatus.Pending);
        moderationTask.AdId.Should().NotBeEmpty();
    }

    [Fact]
    public void Approve_FromPending_ShouldChangeStatus()
    {
        // Arrange
        var adId = Guid.NewGuid();
        var moderationTask = CreateModerationTask(adId);
        var moderatorId = Guid.NewGuid();

        // Act
        moderationTask.Approve(moderatorId);

        // Assert
        moderationTask.Status.Should().Be(ModerationStatus.Approved);
        moderationTask.ModeratorId.Should().Be(moderatorId);

        var domainEvent = moderationTask.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ModerationTaskApprovedEvent>().Subject;

        domainEvent.AdId.Should().Be(adId);
    }

    [Fact]
    public void RejectWithReason_FromPending_ShouldChangeStatus()
    {
        // Arrange
        var adId = Guid.NewGuid();
        var moderationTask = CreateModerationTask(adId);
        const string reason = "da patamy 4ta";

        // Act
        moderationTask.Reject(Guid.NewGuid(), reason);

        // Assert
        moderationTask.Status.Should().Be(ModerationStatus.Rejected);
        moderationTask.RejectionReason.Should().Be(reason);

        var domainEvent = moderationTask.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ModerationTaskRejectedEvent>().Subject;

        domainEvent.AdId.Should().Be(adId);
        domainEvent.Reason.Should().Be(reason);
    }

    [Fact]
    public void RejectWoReason_FromPending_ShouldChangeStatus()
    {
        // Arrange
        var adId = Guid.NewGuid();
        var moderationTask = CreateModerationTask(adId);
        const string? reason = null;

        // Act
        moderationTask.Reject(Guid.NewGuid(), reason);

        // Assert
        moderationTask.Status.Should().Be(ModerationStatus.Rejected);

        var domainEvent = moderationTask.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ModerationTaskRejectedEvent>().Subject;

        domainEvent.AdId.Should().Be(adId);
        domainEvent.Reason.Should().Be(reason);
    }

    [Fact]
    public void Approve_FromApprove_ShouldThrow()
    {
        // Arrange
        var adId = Guid.NewGuid();
        var moderationTask = CreateModerationTask(adId);
        moderationTask.Approve(Guid.NewGuid());

        // Act & Assert
        var act = () => moderationTask.Approve(Guid.NewGuid());
        act.Should().Throw<InvalidOperationException>();
    }
}