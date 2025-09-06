using System.Security.Claims;
using Core.Application.Converters;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.RequestsDTO;
using Core.Application.Models.RequestsDTO.Assignments;
using Core.Application.Models.RequestsDTO.Essays;
using Core.Application.Models.ReturnViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace RepetaitorAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AssignmentController(
    IAssignmentService assignmentService,
    IHttpContextAccessor httpContextAccessor,
    IAICommunicateService aIService,
    ILogger<AssignmentController> logger) : ControllerBase
{
    [Authorize(Roles = "Teacher")]
    [HttpPost("[action]")]
    [ProducesResponseType(typeof(AssignmentBaseModal), 200)]
    public async Task<IResult> CreateNewAssignment(
        [FromBody] CreateNewAssignmentsRequest request)
    {
        logger.LogInformation("CreateNewAssignment request: {request}", JsonConvert.SerializeObject(request));
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
        logger.LogInformation("UpdateAssignment request: {request}", JsonConvert.SerializeObject(request));
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await assignmentService.UpdateAssignment(userId, request);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [HttpPost("[action]")]
    [ProducesResponseType(typeof(List<ResultResponse>), 200)]
    public async Task<IResult> SaveOrSubmitAssignment(
        [FromBody] SaveOrSubmitAssignmentRequest request)
    {
        logger.LogInformation("SaveOrSubmitAssignment request: {request}", JsonConvert.SerializeObject(request));
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await assignmentService.SaveOrSubmitAssignment(userId, request);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("[action]")]
    [ProducesResponseType(typeof(PaginatedResponse<List<GroupAssignmentBaseModal>>), 200)]
    public async Task<IResult> GetGroupAssignments([FromQuery] GetGroupAssignmentsRequest
        request)
    {
        logger.LogInformation("GetGroupAssignments request: {request}", JsonConvert.SerializeObject(request));
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await assignmentService.GetGroupAssignments(userId, request.GroupId, request.PageIndex,
            request.PageSize);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(PaginatedResponse<List<UserAssignmentBaseModal>>), 200)]
    public async Task<IResult> GetUserAssignments(
        [FromQuery] GetUserAssignmentsRequest request)
    {
        logger.LogInformation("GetUserAssignments request: {request}", JsonConvert.SerializeObject(request));
        var resp = await assignmentService.GetUserAssignments(request.UserId,
            request.StatusName,
            request.IsAIAssignment,
            request.PageIndex,
            request.PageSize);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(UserAssignmentModal), 200)]
    public async Task<IResult> GetUserAssignment([FromQuery] int userId,
        [FromQuery] int assignmentId)
    {
        logger.LogInformation("GetUserAssignment request: {assignmentId}", assignmentId);
        var curUserId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await assignmentService.GetUserAssignment(
            curUserId, userId,
            assignmentId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(PaginatedResponse<List<UserAssignmentBaseModal>>), 200)]
    public async Task<IResult>
        GetUserNotSeenEvaluatedAssignments(
            [FromQuery] GetUserNotSeenEvaluatedAssignmentsRequest request)
    {
        logger.LogInformation("GetUserNotSeenEvaluatedAssignments request: {request}",
            JsonConvert.SerializeObject(request));
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
        logger.LogInformation("EvaluateAssignment request: {request}", JsonConvert.SerializeObject(request));
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await assignmentService.EvaluateAssignments(userId, request);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("[action]")]
    [ProducesResponseType(typeof(PaginatedResponse<List<UserAssignmentBaseModal>>), 200)]
    public async Task<IResult> GetNeedEvaluationAssignments(
        [FromQuery] GetNeedEvaluationAssignmentsRequest request)
    {
        logger.LogInformation("GetNeedEvaluationAssignments request: {request}", JsonConvert.SerializeObject(request));
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await assignmentService.GetTeacherAssignments(userId, request.Offset, request.Limit);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(AssignmentBaseModal), 200)]
    public async Task<IResult> GetAssignmentBaseInfoById(
        [FromQuery] int assignmentId)
    {
        logger.LogInformation("GetAssignmentBaseInfoById request: {assignmentId}", assignmentId);
        var resp = await assignmentService.GetAssignmentBaseInfoById(assignmentId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("[action]")]
    [ProducesResponseType(typeof(PaginatedResponse<List<UserAssignmentBaseModal>>), 200)]
    public async Task<IResult> GetUsersTasksByAssignment(
        [FromQuery] GetUsersTasksByAssignmentRequest request)
    {
        logger.LogInformation("GetUsersTasksByAssignment request: {request}", JsonConvert.SerializeObject(request));
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
        logger.LogInformation("DeleteAssignment request: {assignmentId}", assignmentId);
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
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(List<string>), 200)]
    public async Task<IResult> GetUserAssignmentImages(int userId, int assignmentId)
    {
        try
        {
            var resp = await assignmentService.GetUserAssignmentImages(userId, assignmentId);
            return Results.Ok(resp);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    [HttpPut("[action]")]
    [ProducesResponseType(typeof(ResultResponse), 200)]
    public async Task<IResult> ChangeUserAssignmentPublicStatus([FromBody] MakeUserAssignmentPublicRequest request)
    {
        logger.LogInformation("ChangeUserAssignmentPublicStatus request: {request}",
            JsonConvert.SerializeObject(request));
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await assignmentService.MakeUserAssignmentPublic(userId, request.AssignmentId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(List<UserAssignmentBaseModal>), 200)]
    public async Task<IResult> GetPublicUserAssignments([FromQuery] int assignmentId, [FromQuery] int? offset,
        [FromQuery] int? limit)
    {
        logger.LogInformation("GetPublicUserAssignments request: {assignmentId} {offset} {limit}", assignmentId, offset,
            limit);
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await assignmentService.GetPublicUserAssignments(userId, assignmentId, offset, limit);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(ResultResponse), 200)]
    public async Task<IResult> IsAssignmentPublic([FromQuery] int assignmentId)
    {
        logger.LogInformation("IsAssignmentPublic request: {assignmentId}", assignmentId);
        var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
        var resp = await assignmentService.IsAssignmentPublic(userId, assignmentId);
        return ControllerReturnConverter.ConvertToReturnType(resp);
    }
}