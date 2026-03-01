using MediatR;

namespace Pereprodai.Shared.Application;

public interface ICommand : IRequest;

public interface ICommand<out TResult> : IRequest<TResult>;
