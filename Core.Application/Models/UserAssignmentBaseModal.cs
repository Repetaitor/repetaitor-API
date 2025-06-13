namespace Core.Application.Models;

public class UserAssignmentBaseModal
{
    public UserModal? Student { get; set; }
    public StatusBaseModal Status { get; set; }
    public AssignmentBaseModal Assignment { get; set; }
    public bool IsEvaluated { get; set; }
    public DateTime? SubmitDate { get; set; }
    public int TotalScore { get; set; } = -1;
    public int ActualWordCount { get; set; }
    public bool IsPublic { get; set; } = false;
}