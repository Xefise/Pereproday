using Pereprodai.Catalog.Application.DTOs;
using Pereprodai.Shared.Application;

namespace Pereprodai.Catalog.Application.Queries.GetAd;

public record GetAdQuery(Guid AdId, Guid? RequestingUserId = null) : IQuery<AdResponse>;
