using Core.Application.Models;
using Core.Application.Models.DTO.Assignments;
using Core.Application.Models.DTO.Essays;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Application.Interfaces.Services;

public interface IAssignmentService
{
    Task<ResponseViewModel<AssignmentBaseModal>> CreateNewAssignment(CreateNewAssignmentsRequest request);

    Task<ResponseViewModel<AssignmentBaseModal>> UpdateAssignment(UpdateAssignmentRequest request);

    Task<ResponseViewModel<List<AssignmentBaseModal>>> GetGroupAssignments(int userId, int groupId);
    
    Task<ResponseViewModel<List<UserAssignmentBaseModal>>> GetUserAssignments(int userId, int statusId);
    Task<ResponseViewModel<ResultResponse>> SaveOrSubmitAssignment(SaveOrSubmitAssignmentRequest request);
    Task<ResponseViewModel<UserAssignmentModal>> GetUserAssignment(int callerId, int userId, int assignmentId);
    
    Task<ResponseViewModel<List<UserAssignmentBaseModal>>> GetUserNotSeenEvaluatedAssignments(int userId);
    
    Task<ResponseViewModel<List<StatusBaseModal>>> GetEvaluationTextStatuses();
    
    Task<ResponseViewModel<List<StatusBaseModal>>> GetAssignmentStatuses();
    
    Task<ResponseViewModel<ResultResponse>> EvaluateAssignments(EvaluateAssignmentRequest request);
    Task<ResponseViewModel<List<UserAssignmentBaseModal>>> GetTeacherAssignments(int userId);
}