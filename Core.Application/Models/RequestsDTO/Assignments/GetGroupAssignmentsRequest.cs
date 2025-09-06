namespace Core.Application.Models.RequestsDTO.Assignments;

public class GetGroupAssignmentsRequest
{
    public int GroupId { get; set; }
    public int? PageIndex { get; set; }
    public int? PageSize { get; set; }
}