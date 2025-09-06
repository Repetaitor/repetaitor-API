using AutoMapper;
using Core.Application.Models;
using Core.Application.Models.ReturnViewModels;
using Core.Domain.Entities;

namespace Core.Application.Mappers;

public class ChatProfile : Profile
{
    public ChatProfile()
    {
        CreateMap<(Chats, User), PrivateChatViewModel>()
            .ForMember(opt => opt.ChatId, opt => opt.MapFrom(src => src.Item1.Id))
            .ForMember(opt => opt.WithUser, opt => opt.MapFrom(src => src.Item2))
            .ForMember(opt => opt.ChatName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Item1.ChatName)
                ? $"{src.Item2.FirstName} {src.Item2.LastName}"
                : src.Item1.ChatName));
        CreateMap<(Chats, RepetaitorGroup), GroupChatViewModel>()
            .ForMember(opt => opt.ChatId, opt => opt.MapFrom(src => src.Item1.Id))
            .ForMember(opt => opt.GroupId, opt => opt.MapFrom(src => src.Item2.Id))
            .ForMember(opt => opt.ChatName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Item1.ChatName)
                ? src.Item2.GroupName
                : src.Item1.ChatName));
        CreateMap<(ChatMessages, User), ChatMessageViewModel>()
            .ForMember(opt => opt.ByUser, opt => opt.MapFrom(src => src.Item2))
            .ForMember(opt => opt.Message, opt => opt.MapFrom(src => src.Item1.Message))
            .ForMember(opt => opt.SendAt, opt => opt.MapFrom(src => src.Item1.SendAt));
    }
}