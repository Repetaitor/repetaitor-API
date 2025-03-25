namespace Core.Application.Models.DTO.Assignments;

public class GetUsersTasksByAssignmentRequest
{
    public int userId { get; set; }
    public int AssignmentId { get; set; }
    public int statusId { get; set; }
    public int? offset { get; set; }
    public int? limit { get; set; }
}