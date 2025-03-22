using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.DTO.Essays;
using Core.Application.Models.DTO.Groups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg;

namespace RepetaitorAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class GroupController(
    IGroupService groupService,
    IJWTTokenGenerator tokenGenerator,
    IHttpContextAccessor httpContextAccessor)
{
    [Authorize(Roles = "Teacher")]
    [HttpPost("[Action]")]
    public async Task<ResponseViewModel<GroupBaseModal>> CreateGroup([FromBody] CreateGroupRequest request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.UserId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return new ResponseViewModel<GroupBaseModal>()
            {
                Code = -1,
                Message = "You do not have permission.",
            };
        return await groupService.CreateGroup(request.GroupName, request.UserId);
    }

    [Authorize(Roles = "Teacher")]
    [HttpPut("[Action]")]
    public async Task<ResponseViewModel<ResultResponse>> ChangeGroupState([FromBody] ChangeStateGroupRequest request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.UserId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return new ResponseViewModel<ResultResponse>()
            {
                Code = -1,
                Message = "You do not have permission.",
            };
        return await groupService.ChangeGroupState(request.UserId, request.GroupId, request.isActive);
    }

    [HttpPost("[Action]")]
    public async Task<ResponseViewModel<ResultResponse>> AddStudentToGroup([FromBody] AddStudentToGroupRequest request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.UserId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return new ResponseViewModel<ResultResponse>()
            {
                Code = -1,
                Message = "You do not have permission.",
            };
        return await groupService.AddUserToGroup(request.UserId, request.GroupCode);
    }

    [HttpDelete("[Action]")]
    public async Task<ResponseViewModel<ResultResponse>> RemoveStudentFromGroup([FromQuery] int userId,
        [FromQuery] int groupId)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(userId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return new ResponseViewModel<ResultResponse>()
            {
                Code = -1,
                Message = "You do not have permission.",
            };
        return await groupService.RemoveUserFromGroup(groupId, userId);
    }

    [HttpGet("[Action]")]
    public async Task<ResponseViewModel<GroupBaseModal?>> GetStudentGroups([FromQuery] int userId)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(userId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return new ResponseViewModel<GroupBaseModal?>()
            {
                Code = -1,
                Message = "You do not have permission.",
            };
        return await groupService.GetStudentGroup(userId);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("[Action]")]
    public async Task<ResponseViewModel<List<GroupBaseModal>>> GetTeacherGroups([FromQuery] int userId,
        [FromQuery] bool isActive)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(userId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return new ResponseViewModel<List<GroupBaseModal>>()
            {
                Code = -1,
                Message = "You do not have permission.",
            };
        return await groupService.GetTeacherGroups(userId, isActive);
    }

    [Authorize(Roles = "Teacher")]
    [HttpPut("[Action]")]
    public async Task<ResponseViewModel<GroupBaseModal>> UpdateGroupTitle([FromBody] UpdateGroupTitleRequest request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.UserId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return new ResponseViewModel<GroupBaseModal>()
            {
                Code = -1,
                Message = "You do not have permission to add this essay.",
            };
        return await groupService.UpdateGroupTitle(request.UserId, request.GroupId, request.GroupTitle);
    }

    [Authorize(Roles = "Teacher")]
    [HttpPut("[Action]")]
    public async Task<ResponseViewModel<NewGroupCodeResponse>> RegenerateGroupCode(
        [FromBody] RegenerateGroupCodeRequest request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.UserId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return new ResponseViewModel<NewGroupCodeResponse>()
            {
                Code = -1,
                Message = "You do not have permission to add this essay.",
            };
        return await groupService.RegenerateGroupCode(request.UserId, request.GroupId);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("[Action]")]
    public async Task<ResponseViewModel<List<UserModal>>> GetGroupUsers([FromQuery] int userId, [FromQuery] int groupId)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(userId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return new ResponseViewModel<List<UserModal>>()
            {
                Code = -1,
                Message = "You do not have permission to add this essay.",
            };
        return await groupService.GetGroupUsers(userId, groupId);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("[Action]")]
    public async Task<ResponseViewModel<List<GroupBaseModal>>> SearchGroup([FromQuery] string groupName,
        [FromQuery] bool isActive)
    {
        return await groupService.SearchGroup(groupName, isActive);
    }
    [HttpGet("[Action]")]
    public async Task<ResponseViewModel<GroupBaseModal>> GetGroupBaseInfoById([FromQuery] int userId, [FromQuery] int groupId)
    {
        return await groupService.GetGroupBaseInfoById(userId, groupId);
    }
}