using AutoMapper;
using Core.Application.Models;
using Core.Application.Models.ReturnViewModels;
using Core.Domain.Entities;

namespace Core.Application.Mappers;

public class AssignmentProfile : Profile
{
    public AssignmentProfile()
    {
        CreateMap<Assignment, AssignmentBaseModal>();
    }
}