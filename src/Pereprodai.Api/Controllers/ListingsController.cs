using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pereprodai.Api.Extensions;
using Pereprodai.Api.Requests;
using Pereprodai.Api.Requests.Catalog;
using Pereprodai.Catalog.Application.Commands.ArchiveAd;
using Pereprodai.Catalog.Application.Commands.CreateAd;
using Pereprodai.Catalog.Application.Commands.SubmitForModeration;
using Pereprodai.Catalog.Application.Commands.UpdateAd;
using Pereprodai.Catalog.Application.DTOs;
using Pereprodai.Catalog.Application.Queries.GetAd;
using Pereprodai.Catalog.Application.Queries.GetMyAds;
using Pereprodai.Catalog.Application.Services;
using Pereprodai.Catalog.Domain.Enums;
using Pereprodai.Shared.Application.DTOs;

namespace Pereprodai.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ListingsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IViewCountService _viewCountService;

    public ListingsController(IMediator mediator, IViewCountService viewCountService)
    {
        _mediator = mediator;
        _viewCountService = viewCountService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAdRequest request, CancellationToken ct)
    {
        var userId = User.GetUserId();
        var command = new CreateAdCommand(userId, request.Title, request.Description, request.Category,
            request.PriceAmount, request.PriceCurrency, request.City, request.Phone, request.Email);
        var adId = await _mediator.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id = adId }, new { id = adId });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAdRequest request, CancellationToken ct)
    {
        var command = new UpdateAdCommand(id, User.GetUserId(), request.Title, request.Description, request.Category,
            request.PriceAmount, request.PriceCurrency, request.City, request.Phone, request.Email);
        await _mediator.Send(command, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> SubmitForModeration(Guid id, CancellationToken ct)
    {
        var command = new SubmitForModerationCommand(id, User.GetUserId());
        await _mediator.Send(command, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/archive")]
    public async Task<IActionResult> Archive(Guid id, CancellationToken ct)
    {
        var command = new ArchiveAdCommand(id, User.GetUserId());
        await _mediator.Send(command, ct);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdResponse>> GetById(Guid id, CancellationToken ct)
    {
        var optionalUserId = User.GetOptionalUserId();
        var query = new GetAdQuery(id, optionalUserId);
        var result = await _mediator.Send(query, ct);

        if (result.Status == AdStatus.Published && result.UserId != optionalUserId)
            await _viewCountService.IncrementViewCountAsync(id);

        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<ActionResult<PagedResponse<AdListItemResponse>>> GetMy(
        [FromQuery] AdStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = new GetMyAdsQuery(User.GetUserId(), status, page, pageSize);
        var result = await _mediator.Send(query, ct);
        return result;
    }
}
