namespace Core.Application.Models.RequestsDTO.Assignments;

public class GetUsersTasksByAssignmentRequest
{
    public int AssignmentId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public int? Offset { get; set; }
    public int? Limit { get; set; }
}