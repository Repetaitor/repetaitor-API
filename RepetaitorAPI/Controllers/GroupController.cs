using System.Security.Claims;
using Core.Application.Converters;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.RequestsDTO.Groups;
using Core.Application.Models.ReturnViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace RepetaitorAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class GroupController(
    IGroupService groupService,
    IHttpContextAccessor httpContextAccessor,
    ILogger<GroupController> logger) : ControllerBase
{
    [Authorize(Roles = "Teacher")]
    [HttpPost("[Action]")]
    [ProducesResponseType(typeof(GroupBaseModal), 200)]
    public async Task<IResult> CreateGroup([FromBody] CreateGroupRequest request)
    {
        logger.LogInformation("CreateGroup request: {request}", JsonConvert.SerializeObject(request));
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await groupService.CreateGroup(request.GroupName, userId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpDelete("[Action]")]
    public async Task<IResult> DeleteGroup([FromQuery] int groupId)
    {
        logger.LogInformation("DeleteGroup request: {groupId}", groupId);
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await groupService.DeleteGroup(userId, groupId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [HttpPost("[Action]")]
    public async Task<IResult> AddStudentToGroup([FromBody] AddStudentToGroupRequest request)
    {
        logger.LogInformation("AddStudentToGroup request: {request}", JsonConvert.SerializeObject(request));
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await groupService.AddUserToGroup(userId, request.GroupCode);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [HttpDelete("[Action]")]
    public async Task<IResult> RemoveStudentFromGroup(
        [FromQuery] int groupId, [FromQuery] int userId)
    {
        logger.LogInformation("RemoveStudentFromGroup request: {groupId}, {userId}", groupId, userId);
        var callerId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await groupService.RemoveUserFromGroup(callerId, groupId, userId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(GroupBaseModal), 200)]
    public async Task<IResult> GetStudentGroups()
    {
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await groupService.GetStudentGroup(userId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(List<GroupBaseModal>), 200)]
    public async Task<IResult> GetTeacherGroups()
    {
        var userId =
            int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await groupService.GetTeacherGroups(userId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpPut("[Action]")]
    [ProducesResponseType(typeof(GroupBaseModal), 200)]
    public async Task<IResult> UpdateGroupTitle([FromBody] UpdateGroupTitleRequest request)
    {
        logger.LogInformation("UpdateGroupTitle request: {request}", JsonConvert.SerializeObject(request));
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await groupService.UpdateGroupTitle(userId, request.GroupId, request.GroupTitle);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpPut("[Action]")]
    [ProducesResponseType(typeof(NewGroupCodeResponse), 200)]
    public async Task<IResult> RegenerateGroupCode(
        [FromBody] RegenerateGroupCodeRequest request)
    {
        logger.LogInformation("RegenerateGroupCode request: {request}", JsonConvert.SerializeObject(request));
        var resp = await groupService.RegenerateGroupCode(request.UserId, request.GroupId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(List<UserModal>), 200)]
    public async Task<IResult> GetGroupUsers([FromQuery] int groupId)
    {
        logger.LogInformation("GetGroupUsers request: {groupId}", groupId);
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await groupService.GetGroupUsers(userId, groupId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(List<GroupBaseModal>), 200)]
    public async Task<IResult> SearchGroup([FromQuery] string groupName)
    {
        logger.LogInformation("SearchGroup request: {groupName}", groupName);
        var resp = await groupService.SearchGroup(groupName);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(GroupBaseModal), 200)]
    public async Task<IResult> GetGroupBaseInfoById([FromQuery] int groupId)
    {
        logger.LogInformation("GetGroupBaseInfoById request: {groupId}", groupId);
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await groupService.GetGroupBaseInfoById(userId, groupId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }
}