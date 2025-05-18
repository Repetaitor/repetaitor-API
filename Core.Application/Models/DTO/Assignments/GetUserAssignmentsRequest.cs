namespace Core.Application.Models.DTO.Assignments;

public class GetUserAssignmentsRequest
{
    public int UserId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public bool IsAIAssignment { get; set; } = false;
    public int? Offset { get; set; }
    public int? Limit { get; set; }
}