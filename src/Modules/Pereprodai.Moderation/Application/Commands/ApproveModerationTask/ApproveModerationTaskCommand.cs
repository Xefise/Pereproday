using Pereprodai.Shared.Application;

namespace Pereprodai.Moderation.Application.Commands.ApproveModerationTask;

public record ApproveModerationTaskCommand(Guid ModerationTaskId, Guid ModeratorId) : ICommand;