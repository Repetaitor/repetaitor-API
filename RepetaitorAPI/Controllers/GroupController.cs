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
    [ProducesResponseType(typeof(GroupBaseModal), 200)]
    public async Task<IResult> CreateGroup([FromBody] CreateGroupRequest request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.UserId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return Results.Unauthorized();
        var resp = await groupService.CreateGroup(request.GroupName, request.UserId);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpPut("[Action]")]
    public async Task<IResult> ChangeGroupState([FromBody] ChangeStateGroupRequest request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.UserId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return Results.Unauthorized();
        var resp =await groupService.ChangeGroupState(request.UserId, request.GroupId, request.isActive);
        return resp.Result ? Results.Ok() : Results.Problem();
    }

    [HttpPost("[Action]")]
    public async Task<IResult> AddStudentToGroup([FromBody] AddStudentToGroupRequest request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.UserId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return Results.Unauthorized();
        var resp = await groupService.AddUserToGroup(request.UserId, request.GroupCode);
        return resp.Result ? Results.Ok() : Results.Problem();
    }

    [HttpDelete("[Action]")]
    public async Task<IResult> RemoveStudentFromGroup([FromQuery] int userId,
        [FromQuery] int groupId)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(userId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return Results.Unauthorized();
        var resp = await groupService.RemoveUserFromGroup(groupId, userId);
        return resp.Result ? Results.Ok() : Results.Problem();
    }

    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(GroupBaseModal), 200)]
    public async Task<IResult> GetStudentGroups([FromQuery] int userId)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(userId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return Results.Unauthorized();
        var resp= await groupService.GetStudentGroup(userId);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(List<GroupBaseModal>), 200)]
    public async Task<IResult> GetTeacherGroups([FromQuery] int userId,
        [FromQuery] bool isActive)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(userId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return Results.Unauthorized();
        var resp = await groupService.GetTeacherGroups(userId, isActive);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpPut("[Action]")]
    [ProducesResponseType(typeof(GroupBaseModal), 200)]
    public async Task<IResult> UpdateGroupTitle([FromBody] UpdateGroupTitleRequest request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.UserId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return Results.Unauthorized();
        var resp = await groupService.UpdateGroupTitle(request.UserId, request.GroupId, request.GroupTitle);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpPut("[Action]")]
    [ProducesResponseType(typeof(NewGroupCodeResponse), 200)]
    public async Task<IResult> RegenerateGroupCode(
        [FromBody] RegenerateGroupCodeRequest request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.UserId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return Results.Unauthorized();
        var resp = await groupService.RegenerateGroupCode(request.UserId, request.GroupId);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(List<UserModal>), 200)]
    public async Task<IResult> GetGroupUsers([FromQuery] int userId, [FromQuery] int groupId)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(userId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return Results.Unauthorized();
        var resp = await groupService.GetGroupUsers(userId, groupId);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(List<GroupBaseModal>), 200)]
    public async Task<IResult> SearchGroup([FromQuery] string groupName,
        [FromQuery] bool isActive)
    {
        var resp = await groupService.SearchGroup(groupName, isActive);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(GroupBaseModal), 200)]
    public async Task<IResult> GetGroupBaseInfoById([FromQuery] int userId,
        [FromQuery] int groupId)
    {
        var resp = await groupService.GetGroupBaseInfoById(userId, groupId);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }
}