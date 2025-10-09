using AccountabilityInformationSystem.Api.Models.Flow.Ikunks;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Models.Common;

/// <summary>
/// Represents a paginated response containing a collection of items and metadata about the current page, total items,
/// and pagination state.
/// </summary>
/// <remarks>Use this type to return paginated results from queries or APIs, providing both the items for the
/// current page and information needed for navigation, such as page index and total counts. All pagination metadata is
/// included to facilitate client-side paging and navigation.</remarks>
/// <typeparam name="T">The type of items contained in the paginated collection. Must be a reference type.</typeparam>
public sealed class PaginationResponse<T> : ICollectionResponse<T>
where T: class
{
    /// <summary>
    /// Gets the zero-based index of the current page in a paginated collection.
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// Gets the number of items to include on each page when paginating results.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Gets the total number of items represented by the collection or query result.
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Gets the total number of pages available based on the current item count and page size.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Gets a value indicating whether there is a previous page available in the paginated sequence.
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Gets a value indicating whether there is a subsequent page available in the paginated result set.
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Gets the collection of items contained in the current instance.
    /// </summary>
    public List<T> Items { get; init; }

    public static async Task<PaginationResponse<T>> CreateAsync(
        IQueryable<T> source, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken)
    {
        int totalCount = await source.CountAsync(cancellationToken);
        List<T> items = await source
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return new PaginationResponse<T>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items
        };
    }
}
