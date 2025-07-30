using System.Security.Claims;
using Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace RepetaitorAPI.Hubs;
[Authorize]
public class ChatHub(IChatService chatService) : Hub
{
    public async Task SendMessage(string message, string chatId)
    {
        var userId = int.Parse(Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await chatService.AddMessageToChatAsync(userId, int.Parse(chatId), message);
        if (result.Data != null)
        {
            var users = await chatService.GetChatMembers(int.Parse(chatId));
            if (users.Data == null || users.Data.Count == 0)
            {
                return;
            }
            await Clients.Users(users.Data).SendAsync("ReceiveMessage", chatId, result.Data);
        }
    }
}