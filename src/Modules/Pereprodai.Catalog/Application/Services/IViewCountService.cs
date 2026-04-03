namespace Pereprodai.Catalog.Application.Services;

public interface IViewCountService
{
    public Task<Dictionary<Guid, long>> GetViewCountsAsync(IEnumerable<Guid> adIds);
    public Task<long> GetViewCountAsync(Guid adId);
    public Task IncrementViewCountAsync(Guid adId);
    public Task FlushViewCountsAsync();
}