// Services/ISearchService.cs
using System.Linq.Expressions;

namespace PTJ.Services;

public interface ISearchService
{
    /// <summary>
    /// Generic search method that applies database-level filtering for optimal performance
    /// </summary>
    /// <typeparam name="T">Entity type to search</typeparam>
    /// <param name="query">Queryable source</param>
    /// <param name="searchTerm">Search keyword</param>
    /// <param name="selector">Expression to select the field to search (must be translatable to SQL)</param>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="size">Page size</param>
    /// <returns>Paginated search results</returns>
    Task<SearchResult<T>> SearchAsync<T>(
        IQueryable<T> query,
        string searchTerm,
        Expression<Func<T, string>> selector,
        int page = 1,
        int size = 20) where T : class;
}

public class SearchResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
