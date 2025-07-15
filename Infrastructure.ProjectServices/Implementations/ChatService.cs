using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.Models;

namespace Infrastructure.ProjectServices.Implementations;

public class ChatService(IChatRepository chatRepository) : IChatService
{
    public async Task<ResponseView<bool>> AddMessageToChatAsync(int userId, int chatId, string message)
    {
        try
        {
            var sendRes = await chatRepository.AddMessageToChatAsync(userId, chatId, message);

            if (sendRes)
            {
                return new ResponseView<bool>
                {
                    Code = StatusCodesEnum.Success,
                    Data = true
                };
            }

            return new ResponseView<bool>
            {
                Code = StatusCodesEnum.BadRequest,
                Message = "Failed to add message.",
                Data = false
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<bool>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = false
            };
        }
    }

    public async Task<ResponseView<GetUserChatsViewModel>> GetUserChatsAsync(int userId)
    {
        try
        {
            var res = await chatRepository.GetUserChatsAsync(userId);
            return new ResponseView<GetUserChatsViewModel>
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<GetUserChatsViewModel>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<List<ChatMessageViewModel>>> GetChatMessagesAsync(int chatId, int userId)
    {
        try
        {
            var messages = await chatRepository.GetChatMessagesAsync(chatId, userId);
            return new ResponseView<List<ChatMessageViewModel>>
            {
                Code = StatusCodesEnum.Success,
                Data = messages
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<List<ChatMessageViewModel>>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }
}