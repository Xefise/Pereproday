namespace Pereprodai.Shared.Domain.Events.Moderation;

public record ModerationTaskRejectedEvent(Guid AdId, string? Reason) : IDomainEvent;