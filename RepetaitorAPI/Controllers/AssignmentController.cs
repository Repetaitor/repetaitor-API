using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.DTO;
using Core.Application.Models.DTO.Assignments;
using Core.Application.Models.DTO.Essays;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RepetaitorAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AssignmentController(
    IJWTTokenGenerator tokenGenerator,
    IAssignmentService assignmentService,
    IHttpContextAccessor httpContextAccessor)
{
    [Authorize(Roles = "Teacher")]
    [HttpPost("[action]")]
    public async Task<ResponseViewModel<AssignmentBaseModal>> CreateNewAssignment(
        [FromBody] CreateNewAssignmentsRequest request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.UserId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return new ResponseViewModel<AssignmentBaseModal>()
            {
                Code = -1,
                Message = "You do not have permission.",
            };
        return await assignmentService.CreateNewAssignment(request);
    }

    [Authorize(Roles = "Teacher")]
    [HttpPut("[action]")]
    public async Task<ResponseViewModel<AssignmentBaseModal>> UpdateAssignment(
        [FromBody] UpdateAssignmentRequest request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.UserId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return new ResponseViewModel<AssignmentBaseModal>()
            {
                Code = -1,
                Message = "You do not have permission.",
            };
        return await assignmentService.UpdateAssignment(request);
    }

    [HttpPost("[action]")]
    public async Task<ResponseViewModel<ResultResponse>> SaveOrSubmitAssignment(
        [FromBody] SaveOrSubmitAssignmentRequest request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.UserId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return new ResponseViewModel<ResultResponse>()
            {
                Code = -1,
                Message = "You do not have permission.",
            };
        return await assignmentService.SaveOrSubmitAssignment(request);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("[action]")]
    public async Task<ResponseViewModel<CountedResponse<List<AssignmentBaseModal>>>> GetGroupAssignments([FromQuery]
        GetGroupAssignmentsRequest
            request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.UserId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return new ResponseViewModel<CountedResponse<List<AssignmentBaseModal>>>()
            {
                Code = -1,
                Message = "You do not have permission.",
            };
        return await assignmentService.GetGroupAssignments(request.UserId, request.GroupId, request.Offset, request.Limit);
    }

    [HttpGet("[action]")]
    public async Task<ResponseViewModel<CountedResponse<List<UserAssignmentBaseModal>>>> GetUserAssignments(
        [FromQuery] GetUserAssignmentsRequest request)
    {
        return await assignmentService.GetUserAssignments(request.UserId, request.StatusId, request.Offset, request.Limit);
    }

    [HttpGet("[action]")]
    public async Task<ResponseViewModel<UserAssignmentModal>> GetUserAssignment([FromQuery] int userId,
        [FromQuery] int assignmentId)
    {
        return await assignmentService.GetUserAssignment(
            tokenGenerator.GetUserIdFromToken(httpContextAccessor.HttpContext!.Request.Headers.Authorization!), userId,
            assignmentId);
    }

    [HttpGet("[action]")]
    public async Task<ResponseViewModel<CountedResponse<List<UserAssignmentBaseModal>>>>
        GetUserNotSeenEvaluatedAssignments(
            [FromQuery] GetUserNotSeenEvaluatedAssignmentsRequest request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.UserId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return new ResponseViewModel<CountedResponse<List<UserAssignmentBaseModal>>>()
            {
                Code = -1,
                Message = "You do not have permission.",
            };
        return await assignmentService.GetUserNotSeenEvaluatedAssignments(request.UserId, request.Offset, request.Limit);
    }

    [HttpGet("[action]")]
    public async Task<ResponseViewModel<List<StatusBaseModal>>> GetEvaluationTextStatuses()
    {
        return await assignmentService.GetEvaluationTextStatuses();
    }

    [HttpGet("[action]")]
    public async Task<ResponseViewModel<List<StatusBaseModal>>> GetAssignmentStatuses()
    {
        return await assignmentService.GetAssignmentStatuses();
    }

    [Authorize(Roles = "Teacher")]
    [HttpPut("[action]")]
    public async Task<ResponseViewModel<ResultResponse>> EvaluateAssignment(
        [FromBody] EvaluateAssignmentRequest request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.TeacherId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return new ResponseViewModel<ResultResponse>()
            {
                Code = -1,
                Message = "You do not have permission.",
            };
        return await assignmentService.EvaluateAssignments(request);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("[action]")]
    public async Task<ResponseViewModel<CountedResponse<List<UserAssignmentBaseModal>>>> GetNeedEvaluationAssignments(
        [FromQuery] GetNeedEvaluationAssignmentsRequest request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.TeacherId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return new ResponseViewModel<CountedResponse<List<UserAssignmentBaseModal>>>()
            {
                Code = -1,
                Message = "You do not have permission.",
            };
        return await assignmentService.GetTeacherAssignments(request.TeacherId, request.Offset, request.Limit);
    }

    [HttpGet("[action]")]
    public async Task<ResponseViewModel<AssignmentBaseModal>> GetAssignmentBaseInfoById(
        [FromQuery] int assignmentId)
    {
        return await assignmentService.GetAssignmentBaseInfoById(assignmentId);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("[action]")]
    public async Task<ResponseViewModel<CountedResponse<List<UserAssignmentBaseModal>>>> GetUsersTasksByAssignment(
        [FromQuery] GetUsersTasksByAssignmentRequest request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.userId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return new ResponseViewModel<CountedResponse<List<UserAssignmentBaseModal>>>()
            {
                Code = -1,
                Message = "You do not have permission.",
            };
        return await assignmentService.GetAssigmentUsersTasks(request.AssignmentId, request.statusId, request.offset,
            request.limit);
    }
}