namespace Core.Application.Models.DTO.Chats;

public class SendMessageRequestDTO
{
    public int ChatId { get; set; }
    public string Message { get; set; }
}