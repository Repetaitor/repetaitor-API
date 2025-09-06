namespace Core.Application.Models.RequestsDTO.Assignments;

public class GetUserNotSeenEvaluatedAssignmentsRequest
{
    public int? Offset { get; set; }
    public int? Limit { get; set; }
}