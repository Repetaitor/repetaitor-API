using AutoMapper;
using Core.Application.Models;
using Core.Domain.Entities;

namespace Core.Application.Mappers;

public class EssayProfile : Profile
{
    public EssayProfile()
    {
        CreateMap<Essay, EssayModal>();

    }
}