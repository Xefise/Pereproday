using Pereprodai.Shared.Application;

namespace Pereprodai.Catalog.Application.Commands.SubmitForModeration;

public record SubmitForModerationCommand(Guid AdId, Guid UserId) : ICommand;
