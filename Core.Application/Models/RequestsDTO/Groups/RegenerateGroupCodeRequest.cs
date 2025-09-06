namespace Core.Application.Models.RequestsDTO.Groups;

public class RegenerateGroupCodeRequest
{
    public int UserId { get; set; }
    public int GroupId { get; set; }
}