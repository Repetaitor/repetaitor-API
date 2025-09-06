namespace Core.Application.Models.RequestsDTO;

public class PaginatedResponse<T>(int totalCount, int pageSize, int pageNumber, T? result)
{
    public int PageSize { get; set; } = pageSize;
    public int PageNumber { get; set; } = pageNumber;
    public bool HasNextPage => PageNumber * PageSize < TotalCount;
    public bool HasPreviousPage => PageNumber > 1;
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public T? Result { get; set; } = result;
    private int TotalCount { get; set; } = totalCount;
}