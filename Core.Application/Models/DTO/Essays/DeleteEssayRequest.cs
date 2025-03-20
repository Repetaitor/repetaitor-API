namespace Core.Application.Models.DTO.Essays;

public class DeleteEssayRequest
{
    public int UserId { get; set; }
    public int EssayId { get; set; }
}