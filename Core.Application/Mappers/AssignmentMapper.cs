using AutoMapper;
using Core.Application.Models;
using Core.Domain.Entities;

namespace Core.Application.Mappers;

public class AssignmentProfile : Profile
{
    public AssignmentProfile()
    {
        CreateMap<Assignment, AssignmentBaseModal>();
    }
}