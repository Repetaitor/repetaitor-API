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
    public async Task<ResponseView<GroupBaseModal>> CreateGroup(string groupName, string groupCode, int ownerId)
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
            return await GetGroupBaseInfoById(ownerId, group.Id);
        }
        catch (Exception ex)
        {
            return new ResponseView<GroupBaseModal>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<bool>> DeleteGroup(int userId, int groupId)
    {
        try
        {
            var group = await context.Groups.FirstOrDefaultAsync(x => x.Id == groupId);
            if (group == null)
                return new ResponseView<bool>
                {
                    Code = StatusCodesEnum.NotFound,
                    Message = "Group not found",
                    Data = false
                };

            if (group.OwnerId != userId)
            {
                return new ResponseView<bool>
                {
                    Code = StatusCodesEnum.Conflict,
                    Message = "You are not the owner of this group",
                    Data = false
                };
            }

            context.Groups.Remove(group);
            await context.SaveChangesAsync();
            return new ResponseView<bool>
            {
                Code = StatusCodesEnum.Success,
                Message = "Group deleted successfully",
                Data = true
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<bool>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = false
            };
        }
    }

    public async Task<ResponseView<GroupBaseModal>> GetStudentGroup(int userId)
    {
        try
        {
            var groupIds = context.UserGroups.Where(x => x.UserId == userId).Select(x => x.GroupId);
            var group = await context.Groups.Include(x => x.Owner)
                .FirstOrDefaultAsync(x => x.IsAIGroup == false && groupIds.Contains(x.Id));
            if (group == null)
            {
                return new ResponseView<GroupBaseModal>
                {
                    Code = StatusCodesEnum.Conflict,
                    Message = "You are not a member of any teacher group",
                    Data = null
                };
            }

            return new ResponseView<GroupBaseModal>
            {
                Code = StatusCodesEnum.Success,
                Data = GroupMapper.ToGroupModal(group, context.UserGroups.Count(t => t.GroupId == group.Id))
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<GroupBaseModal>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<List<GroupBaseModal>>> GetTeacherGroups(int userId)
    {
        try
        {
            var groups = await context.Groups.Include(x => x.Owner)
                .Where(x => x.OwnerId == userId)
                .OrderByDescending(x => x.CreateDate).Select(x =>
                    GroupMapper.ToGroupModal(x, context.UserGroups.Count(t => t.GroupId == x.Id))).AsNoTracking()
                .ToListAsync();
            return new ResponseView<List<GroupBaseModal>>
            {
                Code = StatusCodesEnum.Success,
                Data = groups
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<List<GroupBaseModal>>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<GroupBaseModal>> UpdateGroupTitle(int userId, int groupId, string groupTitle)
    {
        try
        {
            var group = await context.Groups.Include(x => x.Owner).FirstOrDefaultAsync(x => x.Id == groupId);
            if (group == null)
                return new ResponseView<GroupBaseModal>
                {
                    Code = StatusCodesEnum.NotFound,
                    Message = "Group not found",
                    Data = null
                };
            group.GroupName = groupTitle;
            await context.SaveChangesAsync();
            return new ResponseView<GroupBaseModal>
            {
                Code = StatusCodesEnum.Success,
                Data = GroupMapper.ToGroupModal(group, context.UserGroups.Count(t => t.GroupId == group.Id))
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<GroupBaseModal>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<bool>> UpdateGroupCode(int userId, int groupId, string groupCode)
    {
        try
        {
            var group = await context.Groups.FirstOrDefaultAsync(x => x.Id == groupId);
            if (group == null)
                return new ResponseView<bool>
                {
                    Code = StatusCodesEnum.NotFound,
                    Message = "Group not found",
                    Data = false
                };
            group.GroupCode = groupCode;
            await context.SaveChangesAsync();
            return new ResponseView<bool>
            {
                Code = StatusCodesEnum.Success,
                Message = "Group code updated successfully",
                Data = true
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<bool>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = false
            };
        }
    }

    public async Task<ResponseView<bool>> AddUserToGroup(int userId, string groupCode)
    {
        try
        {
            var group = await context.Groups.FirstOrDefaultAsync(x => x.GroupCode == groupCode);
            if (group == null)
                return new ResponseView<bool>
                {
                    Code = StatusCodesEnum.NotFound,
                    Message = "Group not found",
                    Data = false
                };
            var haveRealStudGroup = await context.UserGroups.Include(g => g.Group)
                .AnyAsync(u => u.UserId == userId && !u.Group.IsAIGroup);
            if (haveRealStudGroup)
                return new ResponseView<bool>
                {
                    Code = StatusCodesEnum.Conflict,
                    Message = "You are already a member of a real student group",
                    Data = false
                };
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
            return new ResponseView<bool>
            {
                Code = StatusCodesEnum.Success,
                Message = "User added to group successfully",
                Data = true
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<bool>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = false
            };
        }
    }

    public async Task<ResponseView<bool>> RemoveUserFromGroup(int callerId, int groupId, int userId)
    {
        try
        {
            var group = await context.Groups.FirstOrDefaultAsync(x => x.Id == groupId);
            if (group == null)
            {
                return new ResponseView<bool>
                {
                    Code = StatusCodesEnum.NotFound,
                    Message = "Group not found",
                    Data = false
                };
            }

            if (callerId != group?.OwnerId && callerId != userId)
                return new ResponseView<bool>
                {
                    Code = StatusCodesEnum.Conflict,
                    Message = "You are not allowed to remove this user from the group",
                    Data = false
                };
            var userGroup =
                await context.UserGroups.FirstOrDefaultAsync(x => x.GroupId == groupId && x.UserId == userId);
            if (userGroup == null)
                return new ResponseView<bool>
                {
                    Code = StatusCodesEnum.Conflict,
                    Message = "User is not a member of this group",
                    Data = false
                };
            await using var conn = new SqlConnection(context.Database.GetConnectionString());
            await using var cmd = new SqlCommand("DeletePendingAssignmentsOfStudents", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@groupId", group!.Id);

            conn.Open();
            cmd.ExecuteNonQuery();

            context.UserGroups.Remove(userGroup);
            await context.SaveChangesAsync();
            return new ResponseView<bool>
            {
                Code = StatusCodesEnum.Success,
                Message = "User removed from group successfully",
                Data = true
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<bool>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = false
            };
        }
    }

    public async Task<ResponseView<GroupBaseModal>> GetGroupBaseInfoById(int userId, int groupId)
    {
        try
        {
            var group = await context.Groups.Include(x => x.Owner).FirstOrDefaultAsync(x => x.Id == groupId);
            if (group == null)
                return new ResponseView<GroupBaseModal>
                {
                    Code = StatusCodesEnum.NotFound,
                    Message = "Group not found",
                    Data = null
                };
            if (group.OwnerId != userId &&
                context.UserGroups.Any(x => x.UserId == userId && groupId == group.Id))
                return new ResponseView<GroupBaseModal>
                {
                    Code = StatusCodesEnum.Conflict,
                    Message = "You are not owner or member of this group",
                    Data = null
                };
            return new ResponseView<GroupBaseModal>
            {
                Code = StatusCodesEnum.Success,
                Data = GroupMapper.ToGroupModal(group, context.UserGroups.Count(t => t.GroupId == group.Id))
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<GroupBaseModal>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<List<GroupBaseModal>>> SearchGroup(string groupName)
    {
        try
        {
            var groups = await context.Groups
                .Include(x => x.Owner)
                .Where(x => EF.Functions.Like(x.GroupName.ToLower(), $"%{groupName.ToLower()}%"))
                .OrderByDescending(x => x.CreateDate).Select(x =>
                    GroupMapper.ToGroupModal(x, context.UserGroups.Count(t => t.GroupId == x.Id))).AsNoTracking()
                .ToListAsync();
            return new ResponseView<List<GroupBaseModal>>
            {
                Code = StatusCodesEnum.Success,
                Data = groups
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<List<GroupBaseModal>>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = []
            };
        }
    }

    public async Task<ResponseView<List<UserModal>>> GetGroupUsers(int userId, int groupId)
    {
        try
        {
            var group = await context.Groups.FirstOrDefaultAsync(x => x.Id == groupId);
            if (group == null)
                return new ResponseView<List<UserModal>>
                {
                    Code = StatusCodesEnum.NotFound,
                    Message = "Group not found",
                    Data = null
                };
            if (group.OwnerId != userId)
                return new ResponseView<List<UserModal>>
                {
                    Code = StatusCodesEnum.Conflict,
                    Message = "You are not owner of this group",
                    Data = null
                };
            var userIds = context.UserGroups.Where(x => x.GroupId == groupId).Select(x => x.UserId);
            var res = await context.Users.Where(x => userIds.Contains(x.Id)).Select(x => new UserModal()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Role = x.Role
            }).ToListAsync();
            return new ResponseView<List<UserModal>>
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<List<UserModal>>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = []
            };
        }
    }
}