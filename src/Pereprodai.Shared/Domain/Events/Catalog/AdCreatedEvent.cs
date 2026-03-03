namespace Pereprodai.Shared.Domain.Events.Catalog;

public record AdCreatedEvent(Guid AdId, Guid UserId) : IDomainEvent;
