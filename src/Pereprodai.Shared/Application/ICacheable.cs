using System.Text.Json.Serialization;

namespace Pereprodai.Shared.Application;

public interface ICacheable
{
    [JsonIgnore] string CachePrefix { get; }
    [JsonIgnore] TimeSpan CacheDuration { get; }
}