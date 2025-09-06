namespace Core.Application.Models.ReturnViewModels;

public class PrivateChatViewModel
{
    public int ChatId { get; set; }
    public UserModal WithUser { get; set; }
    public string ChatName { get; set; } = "";
}