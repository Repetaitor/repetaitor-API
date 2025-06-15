using System.Data;
using Core.Application.Interfaces.Repositories;
using Core.Application.Models;
using Core.Domain.Data;
using Core.Domain.Entities;
using Core.Domain.Mappers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Domain.Repositories;

public class GroupRepository(ApplicationContext context) : IGroupRepository
{
    public async Task<GroupBaseModal> CreateGroup(string groupName, string groupCode, int ownerId)
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
        return await GetGroupBaseInfoById(ownerId, group.Id);
    }

    public async Task<bool> DeleteGroup(int userId, int groupId)
    {
        var group = await context.Groups.FirstOrDefaultAsync(x => x.Id == groupId);
        if (group == null)
            throw new Exception("Group not found");

        if (group.OwnerId != userId)
        {
            throw new Exception("You are not allowed to delete this group");
        }

        context.Groups.Remove(group);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<GroupBaseModal> GetStudentGroup(int userId)
    {
        var groupIds = context.UserGroups.Where(x => x.UserId == userId).Select(x => x.GroupId);
        var group = await context.Groups.Include(x => x.Owner)
            .FirstOrDefaultAsync(x => x.IsAIGroup == false && groupIds.Contains(x.Id));
        if (group == null)
        {
            throw new Exception("You are not a member of any group");
        }

        return GroupMapper.ToGroupModal(group, context.UserGroups.Count(t => t.GroupId == group.Id));
    }

    public async Task<List<GroupBaseModal>> GetTeacherGroups(int userId)
    {
        return await context.Groups.Include(x => x.Owner)
            .Where(x => x.OwnerId == userId)
            .OrderByDescending(x => x.CreateDate).Select(x =>
                GroupMapper.ToGroupModal(x, context.UserGroups.Count(t => t.GroupId == x.Id))).AsNoTracking()
            .ToListAsync();
    }

    public async Task<GroupBaseModal> UpdateGroupTitle(int userId, int groupId, string groupTitle)
    {
        var group = await context.Groups.Include(x => x.Owner).FirstOrDefaultAsync(x => x.Id == groupId);
        if (group == null)
            throw new Exception("Group not found");
        if (group.OwnerId != userId)
            throw new Exception("You are not allowed to update this group");
        group.GroupName = groupTitle;
        await context.SaveChangesAsync();
        return GroupMapper.ToGroupModal(group, context.UserGroups.Count(t => t.GroupId == group.Id));
    }

    public async Task<bool> UpdateGroupCode(int userId, int groupId, string groupCode)
    {
        var group = await context.Groups.FirstOrDefaultAsync(x => x.Id == groupId);
        if (group == null)
            throw new Exception("Group not found");
        group.GroupCode = groupCode;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddUserToGroup(int userId, string groupCode)
    {
        var group = await context.Groups.FirstOrDefaultAsync(x => x.GroupCode == groupCode);
        if (group == null)
            throw new Exception("Group not found");
        var haveRealStudGroup = await context.UserGroups.Include(g => g.Group)
            .AnyAsync(u => u.UserId == userId && !u.Group.IsAIGroup);
        if (haveRealStudGroup)
            throw new Exception("You are already a member of a real student group");
        var userGroup = new UserGroups()
        {
            GroupId = group.Id,
            UserId = userId
        };
        await context.UserGroups.AddAsync(userGroup);
        await using var conn = new SqlConnection(context.Database.GetConnectionString());
        await using var cmd = new SqlCommand("AssignToStudentGroupsAssignments", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@userId", userId);
        cmd.Parameters.AddWithValue("@groupId", group.Id);


        conn.Open();
        cmd.ExecuteNonQuery();


        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveUserFromGroup(int callerId, int groupId, int userId)
    {
        var group = await context.Groups.FirstOrDefaultAsync(x => x.Id == groupId);
        if (group == null)
        {
            throw new Exception("Group not found");
        }

        if (callerId != group?.OwnerId && callerId != userId)
            throw new Exception("You are not allowed to remove this user from the group");
        var userGroup =
            await context.UserGroups.FirstOrDefaultAsync(x => x.GroupId == groupId && x.UserId == userId);
        if (userGroup == null)
            throw new Exception("User is not a member of this group");
        await using var conn = new SqlConnection(context.Database.GetConnectionString());
        await using var cmd = new SqlCommand("DeletePendingAssignmentsOfStudents", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@userId", userId);
        cmd.Parameters.AddWithValue("@groupId", group!.Id);

        conn.Open();
        cmd.ExecuteNonQuery();

        context.UserGroups.Remove(userGroup);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<GroupBaseModal> GetGroupBaseInfoById(int userId, int groupId)
    {
        var group = await context.Groups.Include(x => x.Owner).FirstOrDefaultAsync(x => x.Id == groupId);
        if (group == null)
            throw new Exception("Group not found");
        if (group.OwnerId != userId &&
            context.UserGroups.Any(x => x.UserId == userId && groupId == group.Id))
            throw new Exception("You are not a member of this group");
        return GroupMapper.ToGroupModal(group, context.UserGroups.Count(t => t.GroupId == group.Id));
    }

    public async Task<List<GroupBaseModal>> SearchGroup(string groupName)
    {
        return await context.Groups
            .Include(x => x.Owner)
            .Where(x => EF.Functions.Like(x.GroupName.ToLower(), $"%{groupName.ToLower()}%"))
            .OrderByDescending(x => x.CreateDate).Select(x =>
                GroupMapper.ToGroupModal(x, context.UserGroups.Count(t => t.GroupId == x.Id))).AsNoTracking()
            .ToListAsync();
    }

    public async Task<int> TeacherGroupsCount(int teacherId)
    {
        return await context.Groups.CountAsync(x => x.OwnerId == teacherId && !x.IsAIGroup);
    }

    public async Task<int> TeacherGroupsEnrolledStudentsCount(int teacherId)
    {
        return await context.UserGroups
            .Include(x => x.Group)
            .Where(x => x.Group.OwnerId == teacherId)
            .CountAsync();
    }

    public async Task<List<UserModal>> GetGroupUsers(int userId, int groupId)
    {
        var group = await context.Groups.FirstOrDefaultAsync(x => x.Id == groupId);
        if (group == null)
            throw new Exception("Group not found");
        if (group.OwnerId != userId)
            throw new Exception("You are not allowed to view this group users");
        var userIds = context.UserGroups.Where(x => x.GroupId == groupId).Select(x => x.UserId);
        var res = await context.Users.Where(x => userIds.Contains(x.Id)).Select(x => new UserModal()
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Email = x.Email,
            Role = x.Role
        }).ToListAsync();
        return res;
    }
}