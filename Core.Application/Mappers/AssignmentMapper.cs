using Core.Application.Models;
using Core.Domain.Entities;

namespace Core.Domain.Mappers;

public static class AssignmentMapper
{
    public static AssignmentBaseModal ToAssignmentBaseModal(Assignment assgn)
    {
        return new AssignmentBaseModal()
        {
            Id = assgn.Id,
            Instructions = assgn.Instructions,
            GroupId = assgn.GroupId,
            Creator = UserMapper.ToUserModal(assgn.Creator),
            Essay = EssayMapper.ToEssayModal(assgn.Essay),
            DueDate = assgn.DueDate,
            CreationTime = assgn.CreationTime,
        };
    }
}