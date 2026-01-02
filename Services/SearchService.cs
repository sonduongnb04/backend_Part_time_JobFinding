// Services/SearchService.cs
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace PTJ.Services;

public class SearchService : ISearchService
{
    /// <summary>
    /// Performs search using database-level filtering with EF.Functions.Like
    /// Supports Vietnamese and case-insensitive search with optimal performance
    /// All filtering is done at the database level - no in-memory operations
    /// </summary>
    public async Task<SearchResult<T>> SearchAsync<T>(
        IQueryable<T> query,
        string searchTerm,
        Expression<Func<T, string>> selector,
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

        // Normalize search term (trim)
        var normalizedSearchTerm = searchTerm.Trim();

        // Build database-level filter expression
        // This will be translated to SQL LIKE queries
        var filteredQuery = ApplySearchFilter(query, selector, normalizedSearchTerm);

        // Count total results (executes COUNT query on database)
        var totalCountValue = await filteredQuery.CountAsync();

        // Fetch paginated results (executes SELECT with LIMIT/OFFSET on database)
        var paginatedItems = await filteredQuery
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return new SearchResult<T>
        {
            Items = paginatedItems,
            TotalCount = totalCountValue,
            Page = page,
            PageSize = size
        };
    }

    /// <summary>
    /// Applies search filter at database level using EF.Functions.Like
    /// Supports multiple strategies for flexible matching
    /// </summary>
    private IQueryable<T> ApplySearchFilter<T>(
        IQueryable<T> query,
        Expression<Func<T, string>> selector,
        string searchTerm) where T : class
    {
        // Split search term into words for multi-word search
        var searchWords = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (searchWords.Length == 0)
            return query;

        // Build the filter expression dynamically
        // This creates an expression tree that EF Core can translate to SQL

        // Start with a predicate that checks if the field is not null or empty
        Expression<Func<T, bool>> predicate = BuildSearchPredicate(selector, searchWords);

        return query.Where(predicate);
    }

    /// <summary>
    /// Builds a predicate expression for searching
    /// Combines multiple strategies: exact match, contains, and word-by-word matching
    /// </summary>
    private Expression<Func<T, bool>> BuildSearchPredicate<T>(
        Expression<Func<T, string>> selector,
        string[] searchWords) where T : class
    {
        var parameter = selector.Parameters[0];
        var propertyAccess = selector.Body;

        // Build conditions for each search strategy

        // Strategy 1: Field contains the entire search term (most common case)
        // Translates to: field LIKE '%searchTerm%'
        var fullSearchTerm = string.Join(" ", searchWords);
        var containsFullTerm = BuildContainsExpression(propertyAccess, fullSearchTerm);

        // Strategy 2: Field contains all individual words (for multi-word queries)
        // Translates to: field LIKE '%word1%' AND field LIKE '%word2%' AND ...
        Expression? containsAllWords = null;
        if (searchWords.Length > 1)
        {
            containsAllWords = searchWords
                .Select(word => BuildContainsExpression(propertyAccess, word))
                .Aggregate((acc, expr) => Expression.AndAlso(acc, expr));
        }

        // Combine strategies with OR logic
        var finalCondition = containsAllWords != null
            ? Expression.OrElse(containsFullTerm, containsAllWords)
            : containsFullTerm;

        // Add null/empty check
        var notNullOrEmpty = Expression.Call(
            typeof(string),
            nameof(string.IsNullOrEmpty),
            null,
            propertyAccess
        );
        var isNotNullOrEmpty = Expression.Not(notNullOrEmpty);

        // Combine: field is not null/empty AND (search conditions)
        var finalPredicate = Expression.AndAlso(isNotNullOrEmpty, finalCondition);

        return Expression.Lambda<Func<T, bool>>(finalPredicate, parameter);
    }

    /// <summary>
    /// Builds a "contains" expression using EF.Functions.Like
    /// This is translated to SQL LIKE '%term%' with case-insensitive collation
    /// </summary>
    private Expression BuildContainsExpression(Expression propertyAccess, string searchTerm)
    {
        var pattern = $"%{searchTerm}%";
        var patternExpression = Expression.Constant(pattern);

        // Call EF.Functions.Like(field, pattern)
        // EF Core translates this to SQL: field LIKE pattern COLLATE Latin1_General_CI_AI
        var likeMethod = typeof(DbFunctionsExtensions).GetMethod(
            nameof(DbFunctionsExtensions.Like),
            new[] { typeof(DbFunctions), typeof(string), typeof(string) }
        )!;

        var efFunctionsProperty = Expression.Property(null, typeof(EF), nameof(EF.Functions));

        return Expression.Call(
            likeMethod,
            efFunctionsProperty,
            propertyAccess,
            patternExpression
        );
    }
}
