using Core.Application.Models;
using Core.Application.Models.DTO;
using Core.Application.Models.DTO.Assignments;
using Core.Application.Models.DTO.Essays;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Application.Interfaces.Services;

public interface IAssignmentService
{
    Task<ResponseViewModel<AssignmentBaseModal>> CreateNewAssignment(CreateNewAssignmentsRequest request);

    Task<ResponseViewModel<AssignmentBaseModal>> UpdateAssignment(UpdateAssignmentRequest request);

    Task<ResponseViewModel<CountedResponse<List<AssignmentBaseModal>>>> GetGroupAssignments(int userId, int groupId,int? offset, int? limit);
    
    Task<ResponseViewModel<CountedResponse<List<UserAssignmentBaseModal>>>> GetUserAssignments(int userId, int statusId,int? offset, int? limit);
    Task<ResponseViewModel<ResultResponse>> SaveOrSubmitAssignment(SaveOrSubmitAssignmentRequest request);
    Task<ResponseViewModel<UserAssignmentModal>> GetUserAssignment(int callerId, int userId, int assignmentId);
    
    Task<ResponseViewModel<CountedResponse<List<UserAssignmentBaseModal>>>> GetUserNotSeenEvaluatedAssignments(int userId,int? offset, int? limit);
    
    Task<ResponseViewModel<List<StatusBaseModal>>> GetEvaluationTextStatuses();
    
    Task<ResponseViewModel<List<StatusBaseModal>>> GetAssignmentStatuses();
    
    Task<ResponseViewModel<ResultResponse>> EvaluateAssignments(EvaluateAssignmentRequest request);
    Task<ResponseViewModel<CountedResponse<List<UserAssignmentBaseModal>>>> GetTeacherAssignments(int userId,int? offset, int? limit);
    Task<ResponseViewModel<AssignmentBaseModal>> GetAssignmentBaseInfoById(int assignmentId);
    Task<ResponseViewModel<CountedResponse<List<UserAssignmentBaseModal>>>> GetAssigmentUsersTasks(int assignmentId, int statusId, int? offset, int? limit);
    
}