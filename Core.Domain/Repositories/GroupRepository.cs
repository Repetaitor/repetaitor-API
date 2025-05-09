using Core.Application.Interfaces.Repositories;
using Core.Application.Models;
using Core.Domain.Data;
using Core.Domain.Entities;
using Core.Domain.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Domain.Repositories;

public class GroupRepository(ApplicationContext context, IServiceProvider _serviceProvider) : IGroupRepository
{
    private IAssignmentRepository AssignmentRepository => _serviceProvider.GetRequiredService<IAssignmentRepository>();

    public async Task<GroupBaseModal?> CreateGroup(string groupName, string groupCode, int ownerId)
    {
        try
        {
            var group = new RepetaitorGroup()
            {
                OwnerId = ownerId,
                GroupName = groupName,
                GroupCode = groupCode,
                CreateDate = DateTime.Now,
                IsAIGroup = ownerId == 1019
            };
            await context.AddAsync(group);
            await context.SaveChangesAsync();
            return GroupMapper.ToGroupModal(group, 0);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> DeleteGroup(int userId, int groupId)
    {
        try
        {
            var group = await context.Groups.FirstOrDefaultAsync(x => x.Id == groupId);
            if (group == null || group.OwnerId != userId) return false;
            context.Groups.Remove(group);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<GroupBaseModal?> GetStudentGroup(int userId)
    {
        try
        {
            var groupIds = context.UserGroups.Where(x => x.UserId == userId).Select(x => x.GroupId);
            var group = await context.Groups.FirstOrDefaultAsync(x => groupIds.Contains(x.Id));
            return group != null
                ? GroupMapper.ToGroupModal(group, context.UserGroups.Count(t => t.GroupId == group.Id))
                : null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<List<GroupBaseModal>?> GetTeacherGroups(int userId)
    {
        try
        {
            var groups = await context.Groups.Where(x => x.OwnerId == userId)
                .OrderByDescending(x => x.CreateDate).Select(x =>
                    GroupMapper.ToGroupModal(x, context.UserGroups.Count(t => t.GroupId == x.Id))).AsNoTracking()
                .ToListAsync();
            return groups;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<GroupBaseModal?> UpdateGroupTitle(int userId, int groupId, string groupTitle)
    {
        try
        {
            var group = await context.Groups.FirstOrDefaultAsync(x => x.Id == groupId);
            if (group == null) return null;
            group.GroupName = groupTitle;
            await context.SaveChangesAsync();
            return GroupMapper.ToGroupModal(group, context.UserGroups.Count(t => t.GroupId == group.Id));
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> UpdateGroupCode(int userId, int groupId, string groupCode)
    {
        try
        {
            var group = await context.Groups.FirstOrDefaultAsync(x => x.Id == groupId);
            if (group == null) return false;
            group.GroupCode = groupCode;
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> AddUserToGroup(int userId, string groupCode)
    {
        try
        {
            var group = await context.Groups.FirstOrDefaultAsync(x => x.GroupCode == groupCode);
            if (group == null) return false;
            var haveRealStudGroup = await context.UserGroups.Include(g => g.Group)
                .AnyAsync(u => u.UserId == userId && !u.Group.IsAIGroup);
            if (haveRealStudGroup) return false;
            var userGroup = new UserGroups()
            {
                GroupId = group.Id,
                UserId = userId
            };
            await context.UserGroups.AddAsync(userGroup);
            var rs = await AssignmentRepository.AssignToStudentAllGroupAssignments(userId, group.Id);
            if (!rs) return false;
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> RemoveUserFromGroup(int callerId, int groupId, int userId)
    {
        try
        {
            var group = await context.Groups.FirstOrDefaultAsync(x => x.Id == groupId);
            if(callerId != group?.OwnerId && callerId != userId) return false;
            var userGroup =
                await context.UserGroups.FirstOrDefaultAsync(x => x.GroupId == groupId && x.UserId == userId);
            if (userGroup == null) return false;
            var res = await AssignmentRepository.RemoveGroupAssignmentsForUser(userId, groupId);
            if (!res) return false;
            context.UserGroups.Remove(userGroup);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<GroupBaseModal?> GetGroupBaseInfoById(int userId, int groupId)
    {
        try
        {
            var group = await context.Groups.FirstOrDefaultAsync(x => x.Id == groupId);
            if (group == null || (group.OwnerId != userId &&
                                  context.UserGroups.Any(x => x.UserId == userId && groupId == group.Id))) return null;
            return GroupMapper.ToGroupModal(group, context.UserGroups.Count(t => t.GroupId == group.Id));
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<List<GroupBaseModal>> SearchGroup(string groupName)
    {
        try
        {
            var groups = await context.Groups
                .Where(x => EF.Functions.Like(x.GroupName.ToLower(), $"%{groupName.ToLower()}%")).OrderByDescending(x => x.CreateDate).Select(x =>
                    GroupMapper.ToGroupModal(x, context.UserGroups.Count(t => t.GroupId == x.Id))).AsNoTracking()
                .ToListAsync();
            return groups;
        }
        catch (Exception)
        {
            return [];
        }
    }

    public async Task<List<UserModal>?> GetGroupUsers(int userId, int groupId)
    {
        try
        {
            var group = await context.Groups.FirstOrDefaultAsync(x => x.Id == groupId);
            if (group == null || group.OwnerId != userId) return null;
            var userIds = context.UserGroups.Where(x => x.GroupId == groupId).Select(x => x.UserId);
            return await context.Users.Where(x => userIds.Contains(x.Id)).Select(x => new UserModal()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Role = x.Role
            }).ToListAsync();
        }
        catch (Exception)
        {
            return null;
        }
    }
}