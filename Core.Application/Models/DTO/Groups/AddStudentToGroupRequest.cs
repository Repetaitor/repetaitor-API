namespace Core.Application.Models.DTO.Groups;

public class AddStudentToGroupRequest
{
    public int UserId { get; set; }
    public string GroupCode {get; set;}
}