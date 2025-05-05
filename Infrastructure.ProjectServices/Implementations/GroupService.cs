using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.DTO.Essays;
using Core.Application.Models.DTO.Groups;

namespace Infrastructure.ProjectServices.Implementations;

public class GroupService(IGroupRepository groupRepository) : IGroupService
{
    private static readonly Random Random = new Random();

    private static string GenerateClassCode(int length = 6)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }

    public async Task<GroupBaseModal?> CreateGroup(string groupName, int ownerId)
    {
        var newGroupCode = GenerateClassCode();
        return await groupRepository.CreateGroup(groupName, newGroupCode, ownerId);
    }

    public async Task<ResultResponse> DeleteGroup(int userId, int groupId)
    {
        var res = await groupRepository.DeleteGroup(userId, groupId);

        return new ResultResponse()
        {
            Result = res
        };
    }

    public async Task<GroupBaseModal?> GetStudentGroup(int userId)
    {
        var res = await groupRepository.GetStudentGroup(userId);
        return res is { Id: 0 } ? null : res;
    }

    public async Task<List<GroupBaseModal>?> GetTeacherGroups(int userId)
    {
        return await groupRepository.GetTeacherGroups(userId);
    }

    public async Task<GroupBaseModal?> UpdateGroupTitle(int userId, int groupId, string groupTitle)
    {
        return await groupRepository.UpdateGroupTitle(userId, groupId, groupTitle);
    }

    public async Task<NewGroupCodeResponse?> RegenerateGroupCode(int userId, int groupId)
    {
        var newGroupCode = GenerateClassCode();
        var res = await groupRepository.UpdateGroupCode(userId, groupId, newGroupCode);
        return new NewGroupCodeResponse()
        {
            GroupCode = res ? newGroupCode : ""
        };
    }

    public async Task<ResultResponse> AddUserToGroup(int userId, string groupCode)
    {
        var res = await groupRepository.AddUserToGroup(userId, groupCode);
        return new ResultResponse()
        {
            Result = res
        };
    }

    public async Task<ResultResponse> RemoveUserFromGroup(int callerId, int groupId, int userId)
    {
        var res = await groupRepository.RemoveUserFromGroup(callerId, groupId, userId);
        return new ResultResponse()
        {
            Result = res
        };
    }

    public async Task<List<UserModal>?> GetGroupUsers(int userId, int groupId)
    {
        return await groupRepository.GetGroupUsers(userId, groupId);
    }

    public async Task<List<GroupBaseModal>?> SearchGroup(string groupName)
    {
        return await groupRepository.SearchGroup(groupName);
    }

    public async Task<GroupBaseModal?> GetGroupBaseInfoById(int userId, int groupId)
    {
        return await groupRepository.GetGroupBaseInfoById(userId, groupId);
    }
}