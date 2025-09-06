namespace Core.Application.Models.RequestsDTO.Groups;

public class UpdateGroupTitleRequest
{
    public int GroupId { get; set; }
    public string GroupTitle { get; set; }
}