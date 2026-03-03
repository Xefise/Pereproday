using Pereprodai.Shared.Domain.Events.Catalog.Snapshots;

namespace Pereprodai.Shared.Domain.Events.Catalog;

public record AdSubmittedForModerationEvent(AdSnapshot Snapshot) : IDomainEvent;
