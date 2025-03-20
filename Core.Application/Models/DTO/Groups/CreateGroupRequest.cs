namespace Core.Application.Models.DTO.Groups;

public class CreateGroupRequest
{
    public int UserId { get; set; }
    public string GroupName { get; set; }
}