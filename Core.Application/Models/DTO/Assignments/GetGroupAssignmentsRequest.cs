namespace Core.Application.Models.DTO.Assignments;

public class GetGroupAssignmentsRequest
{
    public int GroupId { get; set; }
    public int? Offset { get; set; }
    public int? Limit { get; set; }
}