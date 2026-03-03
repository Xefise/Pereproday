using Pereprodai.Moderation.Domain.Enums;
using Pereprodai.Shared.Domain;
using Pereprodai.Shared.Domain.Events.Moderation;

namespace Pereprodai.Moderation.Domain.Entities;

public class ModerationTask : AggregateRoot
{
    public ModerationStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }
    public Guid? ModeratorId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ResolvedAt { get; private set; }

    public Guid AdId { get; private set; }

    private ModerationTask() { } // EF Core

    public static ModerationTask Create(Guid adId)
    {
        var moderationTask = new ModerationTask()
        {
            AdId = adId,
            CreatedAt = DateTime.UtcNow,
            Status = ModerationStatus.Pending,
        };

        return moderationTask;
    }

    public void Approve(Guid moderatorId)
    {
        if(Status != ModerationStatus.Pending)
            throw new InvalidOperationException("Can't approve a moderation task that is not in Pending status.");

        Status = ModerationStatus.Approved;
        ResolvedAt = DateTime.UtcNow;
        ModeratorId = moderatorId;

        RaiseDomainEvent(new ModerationTaskApprovedEvent(AdId));
    }

    public void Reject(Guid moderatorId, string? reason)
    {
        if(Status != ModerationStatus.Pending)
            throw new InvalidOperationException("Can't reject a moderation task that is not in Pending status.");

        // пусть отменяют хоть без причины
        RejectionReason = reason;
        ModeratorId = moderatorId;

        Status = ModerationStatus.Rejected;
        ResolvedAt = DateTime.UtcNow;

        RaiseDomainEvent(new ModerationTaskRejectedEvent(AdId, reason));
    }
}