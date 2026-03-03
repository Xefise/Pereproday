using Pereprodai.Moderation.Application.DTOs;
using Pereprodai.Moderation.Domain.Enums;
using Pereprodai.Shared.Application;
using Pereprodai.Shared.Application.DTOs;

namespace Pereprodai.Moderation.Application.Queries.GetModerationQueue;

public record GetModerationQueue(ModerationStatus? Status, int Page = 1, int PageSize = 20) : IQuery<PagedResponse<ModerationTaskResponse>>;