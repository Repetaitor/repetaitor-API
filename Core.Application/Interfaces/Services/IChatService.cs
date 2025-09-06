using Core.Application.Models;
using Core.Application.Models.ReturnViewModels;

namespace Core.Application.Interfaces.Services;

public interface IChatService
{
    public Task<ResponseView<ChatMessageViewModel>> AddMessageToChatAsync(int userId, int chatId, string message);
    public Task<ResponseView<GetUserChatsViewModel>> GetUserChatsAsync(int userId);
    public Task<ResponseView<List<ChatMessageViewModel>>> GetChatMessagesAsync(int chatId, int userId);
    public Task<ResponseView<List<string>>> GetChatMembers(int chatId);
}