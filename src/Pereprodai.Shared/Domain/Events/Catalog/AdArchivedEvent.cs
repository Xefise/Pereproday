namespace Pereprodai.Shared.Domain.Events.Catalog;

public record AdArchivedEvent(Guid AdId) : IDomainEvent;
