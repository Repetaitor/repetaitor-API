namespace Core.Application.Models;

public class UserAssignmentViewForAI
{
    public int UserId { get; set; }
    public int AssignmentId { get; set; }
    public string EssayTitle { get; set; } = string.Empty;
    public string EssayText { get; set; } = string.Empty;
    public int ExpectedWordCount { get; set; } = 0;
}