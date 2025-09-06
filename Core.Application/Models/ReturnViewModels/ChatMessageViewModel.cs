namespace Core.Application.Models.ReturnViewModels;

public class ChatMessageViewModel
{
    public UserModal ByUser { get; set; }
    public string Message { get; set; } = "";
    public DateTime SendAt { get; set; }
}