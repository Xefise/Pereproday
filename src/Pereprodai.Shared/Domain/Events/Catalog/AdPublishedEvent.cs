namespace Pereprodai.Shared.Domain.Events.Catalog;

public record AdPublishedEvent(Guid AdId) : IDomainEvent;
