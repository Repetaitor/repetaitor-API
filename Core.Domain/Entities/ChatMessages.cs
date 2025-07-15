namespace Core.Domain.Entities;

public class ChatMessages
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Message { get; set; }
    public int ChatId { get; set; }
    public DateTime SendAt { get; set; }
}