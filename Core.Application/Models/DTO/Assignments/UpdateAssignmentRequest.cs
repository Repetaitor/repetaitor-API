namespace Core.Application.Models;

public class UpdateAssignmentRequest
{
    public int AssignmentId { get; set; }
    public int EssayId { get; set; }
    public string Instructions { get; set; }
    public DateTime DueDate { get; set; }
}