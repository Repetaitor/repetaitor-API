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

    public async Task<ResponseView<GroupBaseModal>> CreateGroup(string groupName, int ownerId)
    {
        var newGroupCode = GenerateClassCode();
        return await groupRepository.CreateGroup(groupName, newGroupCode, ownerId);
    }

    public async Task<ResponseView<ResultResponse>> DeleteGroup(int userId, int groupId)
    {
        var res = await groupRepository.DeleteGroup(userId, groupId);

        return new ResponseView<ResultResponse>
        {
            Code = res.Code,
            Message = res.Message,
            Data = new ResultResponse
            {
                Result = res.Data
            }
        };
    }

    public async Task<ResponseView<GroupBaseModal>> GetStudentGroup(int userId)
    {
        return await groupRepository.GetStudentGroup(userId);
    }

    public async Task<ResponseView<List<GroupBaseModal>>> GetTeacherGroups(int userId)
    {
        return await groupRepository.GetTeacherGroups(userId);
    }

    public async Task<ResponseView<GroupBaseModal>> UpdateGroupTitle(int userId, int groupId, string groupTitle)
    {
        return await groupRepository.UpdateGroupTitle(userId, groupId, groupTitle);
    }

    public async Task<ResponseView<NewGroupCodeResponse>> RegenerateGroupCode(int userId, int groupId)
    {
        var newGroupCode = GenerateClassCode();
        var res = await groupRepository.UpdateGroupCode(userId, groupId, newGroupCode);
        return new ResponseView<NewGroupCodeResponse>
        {
            Code = res.Code,
            Message = res.Message,
            Data = new NewGroupCodeResponse
            {
                GroupCode = newGroupCode
                
            }
        };
    }

    public async Task<ResponseView<ResultResponse>> AddUserToGroup(int userId, string groupCode)
    {
        var res = await groupRepository.AddUserToGroup(userId, groupCode);
        return new ResponseView<ResultResponse>
        {
            Code = res.Code,
            Message = res.Message,
            Data = new ResultResponse
            {
                Result = res.Data
            }
        };
    }

    public async Task<ResponseView<ResultResponse>> RemoveUserFromGroup(int callerId, int groupId, int userId)
    {
        var res = await groupRepository.RemoveUserFromGroup(callerId, groupId, userId);
        return new ResponseView<ResultResponse>
        {
            Code = res.Code,
            Message = res.Message,
            Data = new ResultResponse
            {
                Result = res.Data
            }
        };
    }

    public async Task<ResponseView<List<UserModal>>> GetGroupUsers(int userId, int groupId)
    {
        return await groupRepository.GetGroupUsers(userId, groupId);
    }

    public async Task<ResponseView<List<GroupBaseModal>>> SearchGroup(string groupName)
    {
        return await groupRepository.SearchGroup(groupName);
    }

    public async Task<ResponseView<GroupBaseModal>> GetGroupBaseInfoById(int userId, int groupId)
    {
        return await groupRepository.GetGroupBaseInfoById(userId, groupId);
    }
}