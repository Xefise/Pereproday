using Pereprodai.Shared.Domain;

namespace Pereprodai.Catalog.Domain.Events;

public record AdArchivedEvent(Guid AdId) : IDomainEvent;
