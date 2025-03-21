using Core.Application.Models;
using Core.Application.Models.DTO.Assignments;
using Core.Application.Models.DTO.Essays;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Application.Interfaces.Services;

public interface IAssignmentService
{
    Task<ResponseViewModel<BaseAssignmentResponse>> CreateNewAssignment(CreateNewAssignmentsRequest request);

    Task<ResponseViewModel<BaseAssignmentResponse>> UpdateAssignment(
        [FromBody] UpdateAssignmentRequest request);

    Task<ResponseViewModel<List<BaseAssignmentResponse>>> GetGroupAssignments(int userId, int groupId);
    
    Task<ResponseViewModel<List<BaseAssignmentResponse>>> GetUserAssignments(int userId, int statusId);
    Task<ResponseViewModel<ResultResponse>> SaveOrSubmitAssignment(SaveOrSubmitAssignmentRequest request);
    Task<ResponseViewModel<UserAssignmentModal>> GetUserAssignment(int callerId, int userId, int assignmentId);
    
    Task<ResponseViewModel<List<BaseAssignmentResponse>>> GetUserNotSeenEvaluatedAssignments(int userId);
    
    Task<ResponseViewModel<List<StatusBaseModal>>> GetEvaluationTextStatuses();
    
    Task<ResponseViewModel<List<StatusBaseModal>>> GetAssignmentStatuses();
    
    Task<ResponseViewModel<ResultResponse>> EvaluateAssignments(EvaluateAssignmentRequest request);
    Task<ResponseViewModel<List<BaseAssignmentResponse>>> GetTeacherAssignments(int userId);
}