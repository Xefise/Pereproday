namespace Pereprodai.Shared.Application.Exceptions;

public class RateLimitExceededException : Exception
{
    public TimeSpan Window { get; }
    public int Limit { get; }
    public RateLimitExceededException(int limit, TimeSpan window) : base($"Rate limit ({limit}) exceeded")
    {
        Limit = limit;
        Window = window;
    }
}