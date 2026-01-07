using System;
using System.Collections.Generic;
using System.Text;

namespace AccountabilityInformationSystem.ConsoleSeeder.DTOs;

internal class PagedResponse<T>
{
    public int Page { get; init; } 
    public int PageSize { get; init; } 
    public int TotalCount { get; init; }
    public int TotalPages { get; init; } 
    public bool HasPreviousPage { get; init; }
    public bool HasNextPage { get; init; }
    public T[] Items { get; init; } = new T[0];
}
