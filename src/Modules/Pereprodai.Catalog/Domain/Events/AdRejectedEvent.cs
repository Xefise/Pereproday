using Pereprodai.Shared.Domain;

namespace Pereprodai.Catalog.Domain.Events;

public record AdRejectedEvent(Guid AdId, string Reason) : IDomainEvent;
