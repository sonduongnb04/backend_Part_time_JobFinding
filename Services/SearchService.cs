// Services/SearchService.cs
using Microsoft.EntityFrameworkCore;

namespace PTJ.Services;

public class SearchService : ISearchService
{

    /// Performs search using EF Core's LIKE operator with fuzzy matching
    /// Supports Vietnamese and case-insensitive search

    public async Task<SearchResult<T>> SearchAsync<T>(
        IQueryable<T> query,
        string searchTerm,
        Func<T, string> selector,
        int page = 1,
        int size = 20) where T : class
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            // If no search term, return paginated results
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            return new SearchResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = size
            };
        }

        // Normalize search term (trim and convert to lower for case-insensitive)
        var normalizedSearchTerm = searchTerm.Trim().ToLower();

        // Fetch all data and apply in-memory filtering
        // Note: For large datasets, consider using full-text search (SQL Server FTS)
        var allData = await query.ToListAsync();

        // Apply fuzzy search algorithm
        var filteredData = allData
            .Where(item =>
            {
                var fieldValue = selector(item);
                if (string.IsNullOrEmpty(fieldValue))
                    return false;

                var normalizedFieldValue = fieldValue.ToLower();

                // Strategy 1: Exact match (highest priority)
                if (normalizedFieldValue == normalizedSearchTerm)
                    return true;

                // Strategy 2: Contains (partial match)
                if (normalizedFieldValue.Contains(normalizedSearchTerm))
                    return true;

                // Strategy 3: Word-by-word matching (for multi-word queries)
                var searchWords = normalizedSearchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var fieldWords = normalizedFieldValue.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                // Check if all search words exist in field words
                var allWordsMatch = searchWords.All(searchWord =>
                    fieldWords.Any(fieldWord => fieldWord.Contains(searchWord)));

                if (allWordsMatch)
                    return true;

                // Strategy 4: Levenshtein distance for typo tolerance (optional)
                // You can implement this for more advanced fuzzy matching

                return false;
            })
            .ToList();

        // Pagination
        var totalCountValue = filteredData.Count;
        var paginatedItems = filteredData
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();

        return new SearchResult<T>
        {
            Items = paginatedItems,
            TotalCount = totalCountValue,
            Page = page,
            PageSize = size
        };
    }


    /// Calculate Levenshtein Distance for fuzzy matching (optional advanced feature)

    private int LevenshteinDistance(string source, string target)
    {
        if (string.IsNullOrEmpty(source))
            return string.IsNullOrEmpty(target) ? 0 : target.Length;

        if (string.IsNullOrEmpty(target))
            return source.Length;

        var sourceLength = source.Length;
        var targetLength = target.Length;
        var distance = new int[sourceLength + 1, targetLength + 1];

        for (var i = 0; i <= sourceLength; i++)
            distance[i, 0] = i;

        for (var j = 0; j <= targetLength; j++)
            distance[0, j] = j;

        for (var i = 1; i <= sourceLength; i++)
        {
            for (var j = 1; j <= targetLength; j++)
            {
                var cost = target[j - 1] == source[i - 1] ? 0 : 1;

                distance[i, j] = Math.Min(
                    Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
                    distance[i - 1, j - 1] + cost);
            }
        }

        return distance[sourceLength, targetLength];
    }
}
