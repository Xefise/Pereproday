using Pereprodai.Moderation.Domain.Entities;
using Pereprodai.Moderation.Infrastructure.ReadModels;
using Pereprodai.Shared.Domain.Events.Catalog.Snapshots;

namespace Pereprodai.Moderation.Application.DTOs;

public static class AdReadModelMapper
{
    public static AdSnapshot ToSnapshot(AdReadModel ad)
    {
        return new AdSnapshot(ad.AdId, ad.UserId, ad.Title, ad.Description, ad.Category, ad.PriceAmount,
            ad.PriceCurrency, ad.LocationCity, ad.ContactInfoPhone, ad.ContactInfoEmail, null, null);
    }

    public static ModerationTaskResponse ToResponse(AdReadModel ad, ModerationTask moderationTask)
    {
        //var moderationTask = ad.ModerationTasks.First();

        return new ModerationTaskResponse(moderationTask.Id, moderationTask.Status, moderationTask.RejectionReason,
            moderationTask.ModeratorId, moderationTask.CreatedAt, moderationTask.ResolvedAt, ToSnapshot(ad));
    }
}