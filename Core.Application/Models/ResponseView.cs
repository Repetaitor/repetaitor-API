namespace Core.Application.Models;

public class ResponseView<T>
{
    public StatusCodesEnum Code { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}