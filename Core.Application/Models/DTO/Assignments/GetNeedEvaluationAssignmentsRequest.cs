namespace Core.Application.Models.DTO.Assignments;

public class GetNeedEvaluationAssignmentsRequest
{
    public int TeacherId { get; set; }
    public int? Offset { get; set; }
    public int? Limit { get; set; }
}