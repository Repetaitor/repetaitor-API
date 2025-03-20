namespace Core.Application.Models.DTO.Groups;

public class ChangeStateGroupRequest
{
    public int UserId { get; set; }
    public int GroupId { get; set; }
    public bool isActive { get; set; }
}