namespace Core.Application.Models.DTO.Assignments;

public class SaveOrSubmitAssignmentRequest
{
    public int AssignmentId { get; set; }
    public string Text { get; set; }
    public int WordCount { get; set; }
    public bool IsSubmitted { get; set; }
}