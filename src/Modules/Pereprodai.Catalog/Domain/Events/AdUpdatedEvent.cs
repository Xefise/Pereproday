using Pereprodai.Shared.Domain;

namespace Pereprodai.Catalog.Domain.Events;

public record AdUpdatedEvent(Guid AdId) : IDomainEvent;
