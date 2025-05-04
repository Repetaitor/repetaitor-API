namespace Core.Application.Models.DTO.Assignments;

public class GetUserNotSeenEvaluatedAssignmentsRequest
{
    public int? Offset { get; set; }
    public int? Limit { get; set; }
}