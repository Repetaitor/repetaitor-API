namespace Core.Application.Models.DTO;

public class CountedResponse<T>
{
    public T? Result { get; set; }
    public int TotalCount { get; set; }
}