using Core.Application.Models;

namespace Core.Application.Interfaces.Services;

public interface IChatService
{
    public Task<ResponseView<bool>> AddMessageToChatAsync(int userId, int chatId, string message);
    public Task<ResponseView<GetUserChatsViewModel>> GetUserChatsAsync(int userId);
    public Task<ResponseView<List<ChatMessageViewModel>>> GetChatMessagesAsync(int chatId, int userId);
}