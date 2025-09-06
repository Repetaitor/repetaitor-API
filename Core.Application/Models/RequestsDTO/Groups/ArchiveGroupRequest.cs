namespace Core.Application.Models.RequestsDTO.Groups;

public class ChangeStateGroupRequest
{
    public int GroupId { get; set; }
    public bool isActive { get; set; }
}