namespace Core.Application.Models.RequestsDTO.Assignments;

public class GetUserAssignmentsRequest
{
    public int UserId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public bool IsAIAssignment { get; set; } = false;
    public int? PageIndex { get; set; }
    public int? PageSize { get; set; }
}