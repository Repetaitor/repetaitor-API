using System.Text.RegularExpressions;
using Core.Application.Models;
using Core.Domain.Entities;

namespace Core.Domain.Mappers;

public class GroupMapper
{
    public static GroupBaseModal ToGroupModal(RepetaitorGroup group, int studentsCount)
    {
        return new GroupBaseModal()
        {
            Id = group.Id,
            OwnerId = group.OwnerId,
            CreateDate = group.CreateDate,
            GroupName = group.GroupName,
            StudentsCount = studentsCount,
            GroupCode = group.GroupCode,
        };
    }
}