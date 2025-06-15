using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.DTO.Essays;
using Core.Application.Models.DTO.Groups;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ProjectServices.Implementations;

public class GroupService(
    IGroupRepository groupRepository,
    ILogger<AssignmentService> logger) : IGroupService
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
        try
        {
            var newGroupCode = GenerateClassCode();
            var res = await groupRepository.CreateGroup(groupName, newGroupCode, ownerId);
            return new ResponseView<GroupBaseModal>
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("SignUp request: {ex}", ex);
            return new ResponseView<GroupBaseModal>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<ResultResponse>> DeleteGroup(int userId, int groupId)
    {
        try
        {
            var res = await groupRepository.DeleteGroup(userId, groupId);

            return new ResponseView<ResultResponse>
            {
                Code = StatusCodesEnum.Success,
                Data = new ResultResponse
                {
                    Result = res
                }
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("SignUp request: {ex}", ex);
            return new ResponseView<ResultResponse>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<GroupBaseModal>> GetStudentGroup(int userId)
    {
        try
        {
            var res = await groupRepository.GetStudentGroup(userId);
            return new ResponseView<GroupBaseModal>
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("SignUp request: {ex}", ex);
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
            var res = await groupRepository.GetTeacherGroups(userId);
            return new ResponseView<List<GroupBaseModal>>
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("SignUp request: {ex}", ex);
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
            var res = await groupRepository.UpdateGroupTitle(userId, groupId, groupTitle);
            return new ResponseView<GroupBaseModal>
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("SignUp request: {ex}", ex);
            return new ResponseView<GroupBaseModal>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<NewGroupCodeResponse>> RegenerateGroupCode(int userId, int groupId)
    {
        try
        {
            var newGroupCode = GenerateClassCode();
            await groupRepository.UpdateGroupCode(userId, groupId, newGroupCode);
            return new ResponseView<NewGroupCodeResponse>
            {
                Code = StatusCodesEnum.Success,
                Data = new NewGroupCodeResponse
                {
                    GroupCode = newGroupCode
                }
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("SignUp request: {ex}", ex);
            return new ResponseView<NewGroupCodeResponse>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<ResultResponse>> AddUserToGroup(int userId, string groupCode)
    {
        try
        {
            var res = await groupRepository.AddUserToGroup(userId, groupCode);
            return new ResponseView<ResultResponse>
            {
                Code = StatusCodesEnum.Success,
                Data = new ResultResponse
                {
                    Result = res
                }
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("SignUp request: {ex}", ex);
            return new ResponseView<ResultResponse>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<ResultResponse>> RemoveUserFromGroup(int callerId, int groupId, int userId)
    {
        try
        {
            var res = await groupRepository.RemoveUserFromGroup(callerId, groupId, userId);
            return new ResponseView<ResultResponse>
            {
                Code = StatusCodesEnum.Success,
                Data = new ResultResponse
                {
                    Result = res
                }
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("SignUp request: {ex}", ex);
            return new ResponseView<ResultResponse>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<List<UserModal>>> GetGroupUsers(int userId, int groupId)
    {
        try
        {
            var res = await groupRepository.GetGroupUsers(userId, groupId);
            return new ResponseView<List<UserModal>>
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("SignUp request: {ex}", ex);
            return new ResponseView<List<UserModal>>
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
            var res = await groupRepository.SearchGroup(groupName);
            return new ResponseView<List<GroupBaseModal>>
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("SignUp request: {ex}", ex);
            return new ResponseView<List<GroupBaseModal>>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<GroupBaseModal>> GetGroupBaseInfoById(int userId, int groupId)
    {
        try
        {
            var res = await groupRepository.GetGroupBaseInfoById(userId, groupId);
            return new ResponseView<GroupBaseModal>
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("SignUp request: {ex}", ex);
            return new ResponseView<GroupBaseModal>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }
}