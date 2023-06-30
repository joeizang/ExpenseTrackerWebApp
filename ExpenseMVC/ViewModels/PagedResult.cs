using ExpenseMVC.Models;

namespace ExpenseMVC;
public record PagedResult<T>(List<T> Items, int TotalCount, int CurrentPage,
    int TotalPages, int PageSize = 7, int PageNumber = 1,
    bool HasNextPage = false, bool HasPreviousPage = false, FilterCriteria CurrentFilter = FilterCriteria.ByDate);