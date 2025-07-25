using Core.Application.Models;

namespace Core.Application.Interfaces.Repositories;

public interface IChatRepository
{
    public Task<int> CreateGroupChatAsync(string chatName, int groupId);
    public Task<int> CreatePrivateChatAsync(string chatName, int firstUserId, int secondUserId);
    public Task<bool> AddUserToGroupChatAsync(int userId, int chatId);
    public Task<bool> AddMessageToChatAsync(int userId, int chatId, string message);
    public Task<bool> RemoveUserFromAllChatAsync(int userId);
    public Task<GetUserChatsViewModel> GetUserChatsAsync(int userId);
    public Task<List<ChatMessageViewModel>> GetChatMessagesAsync(int chatId, int userId);
    public Task<bool> DeleteChatPermanentlyAsync(int chatId, int userId);
}