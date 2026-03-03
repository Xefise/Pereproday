using Pereprodai.Moderation.Domain.Enums;
using Pereprodai.Shared.Domain.Events.Catalog.Snapshots;

namespace Pereprodai.Moderation.Application.DTOs;

public record ModerationTaskResponse(
    Guid Id,
    ModerationStatus Status,
    string? RejectionReason,
    Guid? ModeratorId,
    DateTime CreatedAt,
    DateTime? ResolvedAt, AdSnapshot Snapshot);