namespace Core.Application.Models.DTO.Assignments;

public class GetUserAssignmentsRequest
{
    public int UserId { get; set; }
    public int StatusId { get; set; }
    public int? Offset { get; set; }
    public int? Limit { get; set; }
}