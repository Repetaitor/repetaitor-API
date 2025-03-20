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
    public async Task<ResponseViewModel<GroupBaseModal>> CreateGroup(string groupName, int ownerId)
    {
        var newGroupCode = GenerateClassCode();
        var res = await groupRepository.CreateGroup(groupName, newGroupCode, ownerId);

        return new ResponseViewModel<GroupBaseModal>()
        {
            Code = res != null ? 0 : -1,
            Message = res != null ? "Group Created" : "Group Created Failed",
            Data = res
        };
    }

    public async Task<ResponseViewModel<ResultResponse>> ChangeGroupState(int userId, int groupId, bool isActive)
    {
        var res = await groupRepository.ChangeGroupState(userId, groupId, isActive);

        return new ResponseViewModel<ResultResponse>()
        {
            Code = res ? 0 : -1,
            Message = res ? "Group State Changed" : "Group State Change Failed",
            Data = new ResultResponse()
            {
                Result = res
            }
        };
    }

    public async Task<ResponseViewModel<GroupBaseModal?>> GetStudentGroup(int userId)
    {
        var res = await groupRepository.GetStudentGroup(userId);

        return new ResponseViewModel<GroupBaseModal?>()
        {
            Code = res != null ? 0 : -1,
            Message = res != null ? "" : "Failed To Get Student Group",
            Data = res is { Id: 0 } ? null : res
        };
    }

    public async Task<ResponseViewModel<List<GroupBaseModal>>> GetTeacherGroups(int userId, bool isActive)
    {
        var res = await groupRepository.GetTeacherGroups(userId, isActive);

        return new ResponseViewModel<List<GroupBaseModal>>()
        {
            Code = res != null ? 0 : -1,
            Message = res != null ? "" : "Filed To Get Teacher Groups",
            Data = res ?? []
        };
    }

    public async Task<ResponseViewModel<GroupBaseModal>> UpdateGroupTitle(int userId, int groupId, string groupTitle)
    {
        var res = await groupRepository.UpdateGroupTitle(userId, groupId, groupTitle);

        return new ResponseViewModel<GroupBaseModal>()
        {
            Code =  res != null ? 0 : -1,
            Message = res != null ? "Title Updated" : "Failed to Update Title",
            Data = res
        };
    }

    public async Task<ResponseViewModel<NewGroupCodeResponse>> RegenerateGroupCode(int userId, int groupId)
    {
        var newGroupCode = GenerateClassCode();
        var res = await groupRepository.UpdateGroupCode(userId, groupId, newGroupCode);
        return new ResponseViewModel<NewGroupCodeResponse>()
        {
            Code = res ? 0 : -1,
            Message = res ? "Group Code Updated" : "Failed to Update Group Code",
            Data = new NewGroupCodeResponse()
            {
                GroupCode = res ? newGroupCode : ""
            }
        };
    }

    public async Task<ResponseViewModel<ResultResponse>> AddUserToGroup(int userId, string groupCode)
    {
        var res = await groupRepository.AddUserToGroup(userId, groupCode);
        return new ResponseViewModel<ResultResponse>()
        {
            Code = res ? 0 : -1,
            Message = res ? "User Added" : "Failed to Add User To Group",
            Data = new ResultResponse()
            {
                Result = res
            }
        };
    }

    public async Task<ResponseViewModel<ResultResponse>> RemoveUserFromGroup(int groupId, int userId)
    {
        var res = await groupRepository.RemoveUserFromGroup(groupId, userId);
        return new ResponseViewModel<ResultResponse>()
        {
            Code = res ? 0 : -1,
            Message = res ? "User Removed" : "Failed to Remove User From Group",
            Data = new ResultResponse()
            {
                Result = res
            }
        };
    }

    public async Task<ResponseViewModel<List<UserModal>>> GetGroupUsers(int userId, int groupId)
    {
        var res = await groupRepository.GetGroupUsers(userId, groupId);
        return new ResponseViewModel<List<UserModal>>()
        {
            Code = res != null ? 0 : -1,
            Message = res != null ? "" : "Filed To Get Group Users",
            Data = res ?? []
        };
    }

    public async Task<ResponseViewModel<List<GroupBaseModal>>> SearchGroup(string groupName, bool isActive)
    {
        var res = await groupRepository.SearchGroup(groupName , isActive);
        return new ResponseViewModel<List<GroupBaseModal>>()
        {
            Code = 0,
            Message = "",
            Data = res
        };
    }
}