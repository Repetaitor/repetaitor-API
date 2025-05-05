using System.Security.Claims;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.DTO.Essays;
using Core.Application.Models.DTO.Groups;
using Core.Domain.Entities;
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
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await groupService.CreateGroup(request.GroupName, userId);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpDelete("[Action]")]
    public async Task<IResult> DeleteGroup([FromQuery] int groupId)
    {
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await groupService.DeleteGroup(userId, groupId);
        return resp.Result ? Results.Ok() : Results.Problem();
    }

    [HttpPost("[Action]")]
    public async Task<IResult> AddStudentToGroup([FromBody] AddStudentToGroupRequest request)
    {
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await groupService.AddUserToGroup(userId, request.GroupCode);
        return resp.Result ? Results.Ok() : Results.Problem();
    }

    [HttpDelete("[Action]")]
    public async Task<IResult> RemoveStudentFromGroup(
        [FromQuery] int groupId, [FromQuery] int userId)
    {
        var callerId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await groupService.RemoveUserFromGroup(callerId, groupId, userId);
        return resp.Result ? Results.Ok() : Results.Problem();
    }

    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(GroupBaseModal), 200)]
    public async Task<IResult> GetStudentGroups()
    {
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await groupService.GetStudentGroup(userId);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(List<GroupBaseModal>), 200)]
    public async Task<IResult> GetTeacherGroups()
    {
        var userId =
            int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await groupService.GetTeacherGroups(userId);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpPut("[Action]")]
    [ProducesResponseType(typeof(GroupBaseModal), 200)]
    public async Task<IResult> UpdateGroupTitle([FromBody] UpdateGroupTitleRequest request)
    {
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await groupService.UpdateGroupTitle(userId, request.GroupId, request.GroupTitle);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpPut("[Action]")]
    [ProducesResponseType(typeof(NewGroupCodeResponse), 200)]
    public async Task<IResult> RegenerateGroupCode(
        [FromBody] RegenerateGroupCodeRequest request)
    {
        var resp = await groupService.RegenerateGroupCode(request.UserId, request.GroupId);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(List<UserModal>), 200)]
    public async Task<IResult> GetGroupUsers([FromQuery] int groupId)
    {
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await groupService.GetGroupUsers(userId, groupId);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(List<GroupBaseModal>), 200)]
    public async Task<IResult> SearchGroup([FromQuery] string groupName)
    {
        var resp = await groupService.SearchGroup(groupName);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(GroupBaseModal), 200)]
    public async Task<IResult> GetGroupBaseInfoById([FromQuery] int groupId)
    {
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await groupService.GetGroupBaseInfoById(userId, groupId);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }
}