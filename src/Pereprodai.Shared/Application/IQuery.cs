using MediatR;

namespace Pereprodai.Shared.Application;

public interface IQuery<out TResult> : IRequest<TResult>;
