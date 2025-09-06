using System.Security.Claims;
using Core.Application.Converters;
using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.RequestsDTO.Chats;
using Core.Application.Models.ReturnViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RepetaitorAPI.Controllers;
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ChatController(IChatService chatService, 
    IHttpContextAccessor httpContextAccessor, 
    ILogger<ChatController> logger) : ControllerBase
{
    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(GetUserChatsViewModel), 200)]
    public async Task<IResult> GetUserChats()
    {
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await chatService.GetUserChatsAsync(userId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }
    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(List<ChatMessageViewModel>), 200)]
    public async Task<IResult> GetChatMessages([FromQuery] int chatId)
    {
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await chatService.GetChatMessagesAsync(chatId, userId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }
    [HttpPost("[Action]")]
    [ProducesResponseType(typeof(bool), 200)]
    public async Task<IResult> SendMessage([FromBody] SendMessageRequestDTO request)
    {
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await chatService.AddMessageToChatAsync(userId, request.ChatId, request.Message);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }
}