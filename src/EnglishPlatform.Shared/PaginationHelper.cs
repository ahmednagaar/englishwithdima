using Microsoft.EntityFrameworkCore;

namespace EnglishPlatform.Shared;

/// <summary>
/// Pagination helper for creating paginated results from IQueryable.
/// </summary>
public static class PaginationHelper
{
    public static async Task<PagedList<T>> ToPagedListAsync<T>(
        this IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}

public class PagedList<T>
{
    public List<T> Items { get; }
    public PaginationMeta Meta { get; }

    public PagedList(List<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        Meta = new PaginationMeta
        {
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}

/// <summary>
/// Common query parameters for paginated requests.
/// </summary>
public class PaginationParams
{
    private const int MaxPageSize = 100;
    private int _pageSize = 20;

    public int PageNumber { get; set; } = 1;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}
