namespace Core.Application.Models;

public class GetUserChatsViewModel
{
    public List<GroupChatViewModel> GroupChats { get; set; } = [];
    public List<PrivateChatViewModel> PrivateChats { get; set; } = [];
}