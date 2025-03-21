namespace Core.Application.Models;

public class EvaluationTextCommentModal
{
    public int StatusId { get; set; }
    public int StartIndex { get; set; }
    public int EndIndex { get; set; }
    public string Comment { get; set; }
}