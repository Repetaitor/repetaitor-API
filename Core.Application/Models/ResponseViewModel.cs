namespace Core.Application.Models;

public class ResponseViewModel<T>
{
    public int Code { get; set; } = 0;
    public string Message { get; set; } = String.Empty;
    public T? Data { get; set; } = default!;
}