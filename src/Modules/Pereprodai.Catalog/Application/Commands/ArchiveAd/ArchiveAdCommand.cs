using Pereprodai.Shared.Application;

namespace Pereprodai.Catalog.Application.Commands.ArchiveAd;

public record ArchiveAdCommand(Guid AdId, Guid UserId) : ICommand;
