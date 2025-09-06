using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.ReturnViewModels;
using Core.Domain.Enums;

namespace Infrastructure.ProjectServices.Implementations;

public class ChatService(IChatRepository chatRepository) : IChatService
{
    public async Task<ResponseView<ChatMessageViewModel>> AddMessageToChatAsync(int userId, int chatId, string message)
    {
        try
        {
            var sendRes = await chatRepository.AddMessageToChatAsync(userId, chatId, message);

            return new ResponseView<ChatMessageViewModel>
            {
                Code = StatusCodesEnum.Success,
                Data = sendRes
            };
            
        }
        catch (Exception ex)
        {
            return new ResponseView<ChatMessageViewModel>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
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

    public async Task<ResponseView<List<string>>> GetChatMembers(int chatId)
    {
        try
        {
            var members = await chatRepository.GetChatMembers(chatId);
            return new ResponseView<List<string>>
            {
                Code = StatusCodesEnum.Success,
                Data = members
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<List<string>>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }
}