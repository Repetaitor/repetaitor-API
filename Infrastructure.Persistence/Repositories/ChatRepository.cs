using Core.Application.Interfaces.Repositories;
using Core.Application.Models;
using Core.Domain.Data;
using Core.Domain.Entities;
using Core.Domain.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.Repositories;

public class ChatRepository(ApplicationContext context) : IChatRepository
{
    public async Task<int> CreateGroupChatAsync(string chatName, int groupId)
    {
        var group = await context.Groups.FirstOrDefaultAsync(x => x.Id == groupId);
        if (group == null)
        {
            throw new Exception("Group not found.");
        }

        var chat = new Chats()
        {
            ChatName = chatName,
            ChatType = 1
        };
        await context.Chats.AddAsync(chat);
        await context.SaveChangesAsync();
        group.ChatId = chat.Id;
        context.Groups.Update(group);
        await context.SaveChangesAsync();
        return chat.Id;
    }

    public async Task<int> CreatePrivateChatAsync(string chatName, int firstUserId, int secondUserId)
    {
        var firstUser = await context.Users.FirstOrDefaultAsync(x => x.Id == firstUserId);
        if (firstUser == null)
        {
            throw new Exception("First user not found.");
        }
        var secondUser = await context.Users.FirstOrDefaultAsync(x => x.Id == secondUserId);
        if (secondUser == null)
        {
            throw new Exception("Second user not found.");
        }
        
        var chat = new Chats()
        {
            ChatName = chatName,
            ChatType = 0
        };
        await context.Chats.AddAsync(chat);
        await context.SaveChangesAsync();
        var userChat1 = new UserChats()
        {
            UserId = firstUserId,
            ChatId = chat.Id
        };
        var userChat2 = new UserChats()
        {
            UserId = secondUserId,
            ChatId = chat.Id
        };
        await context.UserChats.AddRangeAsync(userChat1, userChat2);
        await context.SaveChangesAsync();
        return chat.Id;
    }

    public async Task<bool> AddUserToGroupChatAsync(int userId, int chatId)
    {
        var chat = await context.Chats.FirstOrDefaultAsync(x => x.Id == chatId);
        if (chat == null)
        {
            throw new Exception("Chat not found.");
        }

        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var chatUser = new UserChats()
        {
            UserId = userId,
            ChatId = chatId
        };
        await context.UserChats.AddAsync(chatUser);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddMessageToChatAsync(int userId, int chatId, string message)
    {
        if (!await context.UserChats.AnyAsync(x => x.UserId == userId && x.ChatId == chatId))
        {
            throw new Exception("User is not part of the chat.");
        }

        var mes = new ChatMessages()
        {
            Message = message,
            SendAt = DateTime.Now,
            UserId = userId,
            ChatId = chatId
        };
        await context.ChatMessages.AddAsync(mes);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveUserFromAllChatAsync(int userId)
    {
        var userChats = await context.UserChats.Where(x => x.UserId == userId).ToListAsync();
        context.UserChats.RemoveRange(userChats);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<GetUserChatsViewModel> GetUserChatsAsync(int userId)
    {
        var result = new GetUserChatsViewModel();
        var userChats = await context.UserChats.Where(x => x.UserId == userId).ToListAsync();
        foreach (var userChat in userChats)
        {
            var chatInfo = await context.Chats
                .FirstOrDefaultAsync(x => x.Id == userChat.ChatId);
            if (chatInfo == null) continue;
            if (chatInfo.ChatType == 0)
            {
                var withUserId =
                    await context.UserChats.FirstOrDefaultAsync(x => x.ChatId == chatInfo.Id && x.UserId != userId);
                if (withUserId == null) continue;
                var user = await context.Users
                    .FirstOrDefaultAsync(x => x.Id == withUserId.UserId);
                if (user == null) continue;
                var privateChatBaseInfo = new PrivateChatViewModel()
                {
                    ChatId = chatInfo.Id,
                    WithUser = UserMapper.ToUserModal(user),
                    ChatName = string.IsNullOrEmpty(chatInfo.ChatName)
                        ? $"{user.FirstName} {user.LastName}"
                        : chatInfo.ChatName
                };
                result.PrivateChats.Add(privateChatBaseInfo);
            }
            else if (chatInfo.ChatType == 1)
            {
                var group = await context.Groups.FirstOrDefaultAsync(x => x.ChatId == chatInfo.Id);
                if (group == null) continue;
                var groupChatBaseInfo = new GroupChatViewModel()
                {
                    ChatId = chatInfo.Id,
                    ChatName = string.IsNullOrEmpty(chatInfo.ChatName) ? group.GroupName : chatInfo.ChatName
                };
                result.GroupChats.Add(groupChatBaseInfo);
            }
        }

        return result;
    }

    public async Task<List<ChatMessageViewModel>> GetChatMessagesAsync(int chatId, int userId)
    {
        var chatExists = await context.UserChats.AnyAsync(x => x.ChatId == chatId && x.UserId == userId);
        if (!chatExists)
        {
            throw new Exception("User is not part of the chat.");
        }

        var chat = await context.Chats.FirstOrDefaultAsync(x => x.Id == chatId);
        if (chat == null)
        {
            throw new Exception("Chat not found.");
        }

        var messages = await context.ChatMessages
            .Where(x => x.ChatId == chatId)
            .OrderByDescending(x => x.SendAt)
            .ToListAsync();
        var result = new List<ChatMessageViewModel>();
        foreach (var message in messages)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == message.UserId);
            if (user == null) continue;
            var chatMessageViewModel = new ChatMessageViewModel()
            {
                ByUser = UserMapper.ToUserModal(user),
                Message = message.Message,
                SendAt = message.SendAt
            };
            result.Add(chatMessageViewModel);
        }

        return result;
    }

    public async Task<bool> DeleteChatPermanentlyAsync(int chatId, int userId)
    {
        var chat = await context.Chats.FirstOrDefaultAsync(x => x.Id == chatId);
        if (chat == null)
        {
            return true;
        }
        var userChat = await context.UserChats.FirstOrDefaultAsync(x => x.ChatId == chatId && x.UserId == userId);
        if (userChat == null)
        {
            throw new Exception("User is not part of the chat.");
        }

        if (chat.ChatType == 1)
        {
            var group = await context.Groups.FirstOrDefaultAsync(x => x.ChatId == chatId);
            if(group != null && group.OwnerId != userId)
            {
                throw new Exception("Only the group owner can delete the chat.");
            }
        }
        context.ChatMessages.RemoveRange(context.ChatMessages.Where(x => x.ChatId == chatId));
        var userChats = await context.UserChats.Where(x => x.ChatId == chatId).ToListAsync();   
        context.UserChats.RemoveRange(userChats);
        context.Chats.Remove(chat);
        await context.SaveChangesAsync();
        return true;
    }
}