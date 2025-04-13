using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.DTO;
using Core.Application.Models.DTO.Assignments;
using Core.Application.Models.DTO.Essays;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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
    [ProducesResponseType(typeof(AssignmentBaseModal), 200)]  
    public async Task<IResult> CreateNewAssignment(
        [FromBody] CreateNewAssignmentsRequest request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.UserId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return Results.Unauthorized();
        var resp = await assignmentService.CreateNewAssignment(request);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpPut("[action]")]
    [ProducesResponseType(typeof(AssignmentBaseModal), 200)]
    public async Task<IResult> UpdateAssignment(
        [FromBody] UpdateAssignmentRequest request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.UserId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return Results.Unauthorized();
        var resp = await assignmentService.UpdateAssignment(request);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [HttpPost("[action]")]
    public async Task<IResult> SaveOrSubmitAssignment(
        [FromBody] SaveOrSubmitAssignmentRequest request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.UserId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return Results.Unauthorized();
        var resp =  await assignmentService.SaveOrSubmitAssignment(request);
        return resp.Result ? Results.Ok() : Results.Problem();
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("[action]")]
    [ProducesResponseType(typeof(CountedResponse<List<AssignmentBaseModal>>), 200)]
    public async Task<IResult> GetGroupAssignments([FromQuery]
        GetGroupAssignmentsRequest
            request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.UserId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return Results.Unauthorized();
        var resp = await assignmentService.GetGroupAssignments(request.UserId, request.GroupId, request.Offset,
            request.Limit);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(CountedResponse<List<AssignmentBaseModal>>), 200)]
    public async Task<IResult> GetUserAssignments(
        [FromQuery] GetUserAssignmentsRequest request)
    {
        var resp =await assignmentService.GetUserAssignments(request.UserId, request.StatusId, request.Offset,
            request.Limit);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(UserAssignmentBaseModal), 200)]
    public async Task<IResult> GetUserAssignment([FromQuery] int userId,
        [FromQuery] int assignmentId)
    {
        var resp = await assignmentService.GetUserAssignment(
            tokenGenerator.GetUserIdFromToken(httpContextAccessor.HttpContext!.Request.Headers.Authorization!), userId,
            assignmentId);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(CountedResponse<List<UserAssignmentBaseModal>>), 200)]
    public async Task<IResult>
        GetUserNotSeenEvaluatedAssignments(
            [FromQuery] GetUserNotSeenEvaluatedAssignmentsRequest request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.UserId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return Results.Unauthorized();
        var resp = await assignmentService.GetUserNotSeenEvaluatedAssignments(request.UserId, request.Offset,
            request.Limit);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(List<StatusBaseModal>), 200)]
    public async Task<IResult> GetEvaluationTextStatuses()
    {
        var resp = await assignmentService.GetEvaluationTextStatuses();
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(List<StatusBaseModal>), 200)]
    public async Task<IResult> GetAssignmentStatuses()
    {
        var resp = await assignmentService.GetAssignmentStatuses();
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpPut("[action]")]
    public async Task<IResult> EvaluateAssignment(
        [FromBody] EvaluateAssignmentRequest request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.TeacherId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return Results.Unauthorized();
        var resp = await assignmentService.EvaluateAssignments(request);
        return resp.Result ? Results.Ok() : Results.Problem();
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("[action]")]
    [ProducesResponseType(typeof(CountedResponse<List<UserAssignmentBaseModal>>), 200)]
    public async Task<IResult> GetNeedEvaluationAssignments(
        [FromQuery] GetNeedEvaluationAssignmentsRequest request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.TeacherId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return Results.Unauthorized();
        var resp = await assignmentService.GetTeacherAssignments(request.TeacherId, request.Offset, request.Limit);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(AssignmentBaseModal), 200)]
    public async Task<IResult> GetAssignmentBaseInfoById(
        [FromQuery] int assignmentId)
    {
        var resp = await assignmentService.GetAssignmentBaseInfoById(assignmentId);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("[action]")]
    [ProducesResponseType(typeof(CountedResponse<List<UserAssignmentBaseModal>>), 200)]
    public async Task<IResult> GetUsersTasksByAssignment(
        [FromQuery] GetUsersTasksByAssignmentRequest request)
    {
        if (!tokenGenerator.CheckUserIdWithTokenClaims(request.userId,
                httpContextAccessor.HttpContext!.Request.Headers.Authorization!))
            return Results.Unauthorized();
        var resp = await assignmentService.GetAssigmentUsersTasks(request.AssignmentId, request.statusId, request.offset,
            request.limit);
        return resp == null ? Results.Problem() : Results.Ok(resp);
    }
}