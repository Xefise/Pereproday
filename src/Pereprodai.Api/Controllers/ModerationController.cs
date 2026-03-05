using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pereprodai.Api.Extensions;
using Pereprodai.Api.Requests.Moderation;
using Pereprodai.Moderation.Application.Commands.ApproveModerationTask;
using Pereprodai.Moderation.Application.Commands.RejectModerationTask;
using Pereprodai.Moderation.Application.DTOs;
using Pereprodai.Moderation.Application.Queries.GetModerationQueue;
using Pereprodai.Moderation.Domain.Enums;
using Pereprodai.Shared.Application.DTOs;

namespace Pereprodai.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ModerationController : ControllerBase
{
    private readonly IMediator _mediator;

    public ModerationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("queue")]
    public async Task<PagedResponse<ModerationTaskResponse>> GetModerationQueue(
        [FromQuery] ModerationStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var moderations = new GetModerationQueue(status, page, pageSize);
        return await _mediator.Send(moderations, ct);
    }

    [HttpPost("{moderationTaskId:guid}/approve")]
    public async Task<IActionResult> Approve(Guid moderationTaskId, CancellationToken ct)
    {
        var command = new ApproveModerationTaskCommand(moderationTaskId, User.GetUserId());
        await _mediator.Send(command, ct);
        return NoContent();
    }

    [HttpPost("{moderationTaskId:guid}/reject")]
    public async Task<IActionResult> Reject([FromRoute] Guid moderationTaskId, [FromBody] RejectModerationRequest request, CancellationToken ct)
    {
        var command = new RejectModerationTaskCommand(moderationTaskId, User.GetUserId(), request.Reason);
        await _mediator.Send(command, ct);
        return NoContent();
    }
}