using Pereprodai.Shared.Domain;

namespace Pereprodai.Catalog.Domain.Events;

public record AdCreatedEvent(Guid AdId, Guid UserId) : IDomainEvent;
