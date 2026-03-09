using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pereprodai.Search.Application.Queries.SearchAds;

namespace Pereprodai.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public SearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<SearchResult>> Search([FromQuery] SearchAdsQuery query, CancellationToken ct = default)
    {
        return await _mediator.Send(query, ct);
    }
}