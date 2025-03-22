using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
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
    public async Task<ResponseViewModel<List<AssignmentBaseModal>>> GetGroupAssignments([FromQuery] int userId,
        [FromQuery] int groupId)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(userId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return new ResponseViewModel<List<AssignmentBaseModal>>()
            {
                Code = -1,
                Message = "You do not have permission.",
            };
        return await assignmentService.GetGroupAssignments(userId, groupId);
    }

    [HttpGet("[action]")]
    public async Task<ResponseViewModel<List<UserAssignmentBaseModal>>> GetUserAssignments([FromQuery] int userId,
        [FromQuery] int statusId)
    {
        return await assignmentService.GetUserAssignments(userId, statusId);
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
    public async Task<ResponseViewModel<List<UserAssignmentBaseModal>>> GetUserNotSeenEvaluatedAssignments(
        [FromQuery] int userId)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(userId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return new ResponseViewModel<List<UserAssignmentBaseModal>>()
            {
                Code = -1,
                Message = "You do not have permission.",
            };
        return await assignmentService.GetUserNotSeenEvaluatedAssignments(userId);
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
    public async Task<ResponseViewModel<List<UserAssignmentBaseModal>>> GetNeedEvaluationAssignments(
        [FromQuery] int teacherId)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(teacherId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return new ResponseViewModel<List<UserAssignmentBaseModal>>()
            {
                Code = -1,
                Message = "You do not have permission.",
            };
        return await assignmentService.GetTeacherAssignments(teacherId);
    }
}