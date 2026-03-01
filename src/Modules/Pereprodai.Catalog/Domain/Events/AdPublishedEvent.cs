using Pereprodai.Shared.Domain;

namespace Pereprodai.Catalog.Domain.Events;

public record AdPublishedEvent(Guid AdId) : IDomainEvent;
