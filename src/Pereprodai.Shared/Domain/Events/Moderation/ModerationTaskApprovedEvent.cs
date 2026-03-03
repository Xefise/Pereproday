namespace Pereprodai.Shared.Domain.Events.Moderation;

public record ModerationTaskApprovedEvent(Guid AdId) : IDomainEvent;