using Pereprodai.Catalog.Domain.Entities;

namespace Pereprodai.Catalog.Application.DTOs;

public static class AdMapper
{
    public static AdResponse ToResponse(Ad ad)
    {
        return new AdResponse(ad.Id, ad.UserId, ad.Title, ad.Description, ad.Category, ad.Price.Amount,
            ad.Price.Currency, ad.Location.City, ad.ContactInfo.Phone, ad.ContactInfo.Email, ad.Status, ad.CreatedAt,
            ad.UpdatedAt);
    }

    public static AdListItemResponse ToListItemResponse(Ad ad)
    {
        return new AdListItemResponse(ad.Id, ad.Title, ad.Category, ad.Price.Amount, ad.Price.Currency, ad.Location.City,
            ad.Status, ad.CreatedAt);
    }
}
