using AutoMapper;
using Core.Application.Models;
using Core.Application.Models.ReturnViewModels;
using Core.Domain.Entities;

namespace Core.Application.Mappers;

public class EssayProfile : Profile
{
    public EssayProfile()
    {
        CreateMap<Essay, EssayModal>();

    }
}