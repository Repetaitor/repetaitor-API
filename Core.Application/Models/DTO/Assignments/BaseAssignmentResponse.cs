namespace Core.Application.Models.DTO.Assignments;

public class BaseAssignmentResponse
{
    public int AssignmentId { get; set; }
    public string AssignmentName { get; set; }
    public string Instructions { get; set; }
    public StatusBaseModal? Status { get; set; }
    public UserModal? Creator { get; set; }
    public DateTime DueDate { get; set; }
    public int ExpectedWordCount { get; set; }
    public int ActualWordCount { get; set; }
    public int TotalScore { get; set; } = -1;
}