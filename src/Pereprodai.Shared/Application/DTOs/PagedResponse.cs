namespace Pereprodai.Shared.Application.DTOs;

public record PagedResponse<T>(
    List<T> Items,
    long TotalCount,
    int Page,
    int PageSize);
