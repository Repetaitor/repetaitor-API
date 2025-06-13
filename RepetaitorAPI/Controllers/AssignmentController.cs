using System.Security.Claims;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.DTO;
using Core.Application.Models.DTO.Assignments;
using Core.Application.Models.DTO.Essays;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RepetaitorAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AssignmentController(
    IAssignmentService assignmentService,
    IHttpContextAccessor httpContextAccessor,
    IAICommunicateService aIService) : ControllerBase
{
    [Authorize(Roles = "Teacher")]
    [HttpPost("[action]")]
    [ProducesResponseType(typeof(AssignmentBaseModal), 200)]
    public async Task<IResult> CreateNewAssignment(
        [FromBody] CreateNewAssignmentsRequest request)
    {
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await assignmentService.CreateNewAssignment(userId, request);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpPut("[action]")]
    [ProducesResponseType(typeof(AssignmentBaseModal), 200)]
    public async Task<IResult> UpdateAssignment(
        [FromBody] UpdateAssignmentRequest request)
    {
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await assignmentService.UpdateAssignment(userId, request);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [HttpPost("[action]")]
    [ProducesResponseType(typeof(List<ResultResponse>), 200)]
    public async Task<IResult> SaveOrSubmitAssignment(
        [FromBody] SaveOrSubmitAssignmentRequest request)
    {
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await assignmentService.SaveOrSubmitAssignment(userId, request);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("[action]")]
    [ProducesResponseType(typeof(CountedResponse<List<AssignmentBaseModal>>), 200)]
    public async Task<IResult> GetGroupAssignments([FromQuery] GetGroupAssignmentsRequest
        request)
    {
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await assignmentService.GetGroupAssignments(userId, request.GroupId, request.Offset,
            request.Limit);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(CountedResponse<List<UserAssignmentBaseModal>>), 200)]
    public async Task<IResult> GetUserAssignments(
        [FromQuery] GetUserAssignmentsRequest request)
    {
        var resp = await assignmentService.GetUserAssignments(request.UserId, 
            request.StatusName,
            request.IsAIAssignment,
            request.Offset,
            request.Limit);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(UserAssignmentModal), 200)]
    public async Task<IResult> GetUserAssignment([FromQuery] int userId,
        [FromQuery] int assignmentId)
    {
        var curUserId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await assignmentService.GetUserAssignment(
            curUserId, userId,
            assignmentId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(CountedResponse<List<UserAssignmentBaseModal>>), 200)]
    public async Task<IResult>
        GetUserNotSeenEvaluatedAssignments(
            [FromQuery] GetUserNotSeenEvaluatedAssignmentsRequest request)
    {
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await assignmentService.GetUserNotSeenEvaluatedAssignments(userId, request.Offset,
            request.Limit);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(List<StatusBaseModal>), 200)]
    public async Task<IResult> GetEvaluationTextStatuses()
    {
        var resp = await assignmentService.GetEvaluationTextStatuses();
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(List<StatusBaseModal>), 200)]
    public async Task<IResult> GetAssignmentStatuses()
    {
        var resp = await assignmentService.GetAssignmentStatuses();
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpPut("[action]")]
    [ProducesResponseType(typeof(List<ResultResponse>), 200)]
    public async Task<IResult> EvaluateAssignment(
        [FromBody] EvaluateAssignmentRequest request)
    {
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await assignmentService.EvaluateAssignments(userId, request);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("[action]")]
    [ProducesResponseType(typeof(CountedResponse<List<UserAssignmentBaseModal>>), 200)]
    public async Task<IResult> GetNeedEvaluationAssignments(
        [FromQuery] GetNeedEvaluationAssignmentsRequest request)
    {
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await assignmentService.GetTeacherAssignments(userId, request.Offset, request.Limit);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(AssignmentBaseModal), 200)]
    public async Task<IResult> GetAssignmentBaseInfoById(
        [FromQuery] int assignmentId)
    {
        var resp = await assignmentService.GetAssignmentBaseInfoById(assignmentId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("[action]")]
    [ProducesResponseType(typeof(CountedResponse<List<UserAssignmentBaseModal>>), 200)]
    public async Task<IResult> GetUsersTasksByAssignment(
        [FromQuery] GetUsersTasksByAssignmentRequest request)
    {
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await assignmentService.GetAssigmentUsersTasks(userId, request.AssignmentId, request.StatusName,
            request.Offset,
            request.Limit);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }
    [Authorize(Roles = "Teacher")]
    [HttpDelete("[action]")]
    [ProducesResponseType(typeof(AssignmentBaseModal), 200)]
    public async Task<IResult> DeleteAssignment([FromQuery] int assignmentId)
    {
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await assignmentService.DeleteAssignment(userId, assignmentId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }
    [HttpPost("[action]")]
    [ProducesResponseType(typeof(string), 200)]
    public async Task<IResult> GetTextFromImage([FromBody] string[] imagesBase64)
    {
        try
        {
            var resp = await aIService.GetEssayTextFromImage([..imagesBase64]);
            return Results.Ok(resp);
        } catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
    [HttpGet("[action]")]
    [ProducesResponseType(typeof(List<string>), 200)]
    public IResult GetUserAssignmentImages(int userId, int assignmentId)
    {
        try
        {
            var resp = assignmentService.GetUserAssignmentImages(userId, assignmentId);
            return Results.Ok(resp);
        } catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
    [HttpPut("[action]")]
    [ProducesResponseType(typeof(ResultResponse), 200)]
    public async Task<IResult> ChangeUserAssignmentPublicStatus([FromBody] MakeUserAssignmentPublicRequest request)
    {
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await assignmentService.MakeUserAssignmentPublic(userId, request.AssignmentId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }
    [HttpGet("[action]")]
    [ProducesResponseType(typeof(List<UserAssignmentBaseModal>), 200)]
    public async Task<IResult> GetPublicUserAssignments([FromQuery] int assignmentId, [FromQuery] int? offset, [FromQuery] int? limit)
    {
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await assignmentService.GetPublicUserAssignments(userId, assignmentId, offset, limit);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }
    [HttpGet("[action]")]
    [ProducesResponseType(typeof(ResultResponse), 200)]
    public async Task<IResult> IsAssignmentPublic([FromQuery] int assignmentId)
    {
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await assignmentService.IsAssignmentPublic(userId, assignmentId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }
}