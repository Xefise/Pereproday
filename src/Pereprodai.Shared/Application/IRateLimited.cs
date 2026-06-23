namespace Pereprodai.Shared.Application;

public interface IRateLimited
{
    int Limit { get; }
    TimeSpan Window { get; }
    string RateLimitKey { get; }
}