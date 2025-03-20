namespace Core.Application.Models.DTO.Groups;

public class RegenerateGroupCodeRequest
{
    public int UserId { get; set; }
    public int GroupId { get; set; }
}