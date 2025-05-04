namespace Core.Application.Models.DTO.Assignments;

public class CreateNewAssignmentsRequest
{
    public int EssayId { get; set; }
    public int GroupId { get; set; }
    public string Instructions { get; set; }
    public DateTime DueDate { get; set; }
}