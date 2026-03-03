using Pereprodai.Shared.Application;

namespace Pereprodai.Moderation.Application.Commands.RejectModerationTask;

public record RejectModerationTaskCommand(Guid ModerationTaskId, Guid ModeratorId, string? Reason) : ICommand;