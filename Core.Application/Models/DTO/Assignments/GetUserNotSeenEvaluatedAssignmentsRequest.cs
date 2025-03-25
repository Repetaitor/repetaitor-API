namespace Core.Application.Models.DTO.Assignments;

public class GetUserNotSeenEvaluatedAssignmentsRequest
{
    public int UserId { get; set; }
    public int? Offset { get; set; }
    public int? Limit { get; set; }
}