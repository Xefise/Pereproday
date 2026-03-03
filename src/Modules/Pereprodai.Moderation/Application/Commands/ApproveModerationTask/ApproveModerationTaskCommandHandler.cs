using MediatR;
using Pereprodai.Moderation.Domain.Repositories;

namespace Pereprodai.Moderation.Application.Commands.ApproveModerationTask;

public class ApproveModerationTaskCommandHandler : IRequestHandler<ApproveModerationTaskCommand>
{
    private readonly IModerationTaskRepository _moderationTaskRepository;

    public ApproveModerationTaskCommandHandler(IModerationTaskRepository moderationTaskRepository)
    {
        _moderationTaskRepository = moderationTaskRepository;
    }

    public async Task Handle(ApproveModerationTaskCommand command, CancellationToken ct)
    {
        var moderation = await _moderationTaskRepository.GetByIdAsync(command.ModerationTaskId);
        if(moderation is null) throw new KeyNotFoundException();
        moderation.Approve(command.ModeratorId);

        await _moderationTaskRepository.SaveChangesAsync(ct);
    }
}