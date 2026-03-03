using MediatR;
using Pereprodai.Moderation.Domain.Repositories;

namespace Pereprodai.Moderation.Application.Commands.RejectModerationTask;

public class RejectModerationTaskCommandHandler : IRequestHandler<RejectModerationTaskCommand>
{
    private readonly IModerationTaskRepository _moderationTaskRepository;

    public RejectModerationTaskCommandHandler(IModerationTaskRepository moderationTaskRepository)
    {
        _moderationTaskRepository = moderationTaskRepository;
    }

    public async Task Handle(RejectModerationTaskCommand command, CancellationToken ct)
    {
        var moderation = await _moderationTaskRepository.GetByIdAsync(command.ModerationTaskId);
        if(moderation is null) throw new KeyNotFoundException();
        moderation.Reject(command.ModeratorId, command.Reason);

        await _moderationTaskRepository.SaveChangesAsync(ct);
    }
}