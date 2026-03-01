using Pereprodai.Shared.Domain;

namespace Pereprodai.Catalog.Domain.Events;

public record AdSubmittedForModerationEvent(Guid AdId) : IDomainEvent;
