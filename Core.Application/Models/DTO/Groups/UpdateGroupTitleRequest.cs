namespace Core.Application.Models.DTO.Groups;

public class UpdateGroupTitleRequest
{
    public int GroupId { get; set; }
    public string GroupTitle { get; set; }
}