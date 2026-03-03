using MediatR;
using Microsoft.EntityFrameworkCore;
using Pereprodai.Moderation.Application.DTOs;
using Pereprodai.Moderation.Domain.Enums;
using Pereprodai.Moderation.Infrastructure;
using Pereprodai.Shared.Application.DTOs;
using Pereprodai.Shared.Domain.Events.Catalog.Snapshots;

namespace Pereprodai.Moderation.Application.Queries.GetModerationQueue;

public class GetModerationQueueHandler : IRequestHandler<GetModerationQueue, PagedResponse<ModerationTaskResponse>>
{
    private readonly ModerationDbContext _context;

    public GetModerationQueueHandler(ModerationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<ModerationTaskResponse>> Handle(GetModerationQueue request, CancellationToken ct)
    {
        var page = request.Page;
        var pageSize = request.PageSize;

        var query = _context.ModerationTasks
            .Where(t => t.Status == ModerationStatus.Pending);
        var totalCount = await query.CountAsync(ct);

        var list = await query
            .Join(_context.AdReadModels,
                task => task.AdId,
                ad => ad.AdId,
                (task, ad) => new ModerationTaskResponse(
                    task.Id, task.Status, task.RejectionReason,
                    task.ModeratorId, task.CreatedAt, task.ResolvedAt,
                    new AdSnapshot(ad.AdId, ad.UserId, ad.Title, ad.Description,
                        ad.Category, ad.PriceAmount, ad.PriceCurrency,
                        ad.LocationCity, ad.ContactInfoPhone, ad.ContactInfoEmail)))
            .OrderBy(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResponse<ModerationTaskResponse>(list, page, pageSize, totalCount);
    }
}