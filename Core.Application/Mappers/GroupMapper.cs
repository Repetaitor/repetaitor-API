using AutoMapper;
using Core.Application.Models;
using Core.Application.Models.ReturnViewModels;
using Core.Domain.Entities;

namespace Core.Application.Mappers;

public class GroupProfile : Profile
{
    public GroupProfile()
    {
        CreateMap<(RepetaitorGroup, int), GroupBaseModal>()
            .ForMember(dest => dest.StudentsCount, opt => opt.MapFrom(src => src.Item2))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Item1.Id))
            .ForMember(dest => dest.GroupCode, opt => opt.MapFrom(src => src.Item1.GroupCode))
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Item1.GroupName))
            .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.Item1.CreateDate))
            .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Item1.Owner));
    }
}