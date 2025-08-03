using AutoMapper;
using Core.Application.Models;
using Core.Domain.Entities;

namespace Core.Application.Mappers;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserModal>();
    }
}