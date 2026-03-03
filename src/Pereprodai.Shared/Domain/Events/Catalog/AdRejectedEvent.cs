namespace Pereprodai.Shared.Domain.Events.Catalog;

public record AdRejectedEvent(Guid AdId, string? Reason) : IDomainEvent;
