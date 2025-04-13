using Core.Application.Models;
using Core.Application.Models.DTO;
using Core.Application.Models.DTO.Assignments;
using Core.Application.Models.DTO.Essays;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Application.Interfaces.Services;

public interface IAssignmentService
{
    Task<AssignmentBaseModal?> CreateNewAssignment(CreateNewAssignmentsRequest request);

    Task<AssignmentBaseModal?> UpdateAssignment(UpdateAssignmentRequest request);

    Task<CountedResponse<List<AssignmentBaseModal>>?> GetGroupAssignments(int userId, int groupId,
        int? offset, int? limit);

    Task<CountedResponse<List<UserAssignmentBaseModal>>?> GetUserAssignments(int userId, int statusId,
        int? offset, int? limit);

    Task<ResultResponse> SaveOrSubmitAssignment(SaveOrSubmitAssignmentRequest request);
    Task<UserAssignmentModal?> GetUserAssignment(int callerId, int userId, int assignmentId);

    Task<CountedResponse<List<UserAssignmentBaseModal>>?> GetUserNotSeenEvaluatedAssignments(
        int userId, int? offset, int? limit);

    Task<List<StatusBaseModal>?> GetEvaluationTextStatuses();

    Task<List<StatusBaseModal>?> GetAssignmentStatuses();

    Task<ResultResponse> EvaluateAssignments(EvaluateAssignmentRequest request);

    Task<CountedResponse<List<UserAssignmentBaseModal>>?> GetTeacherAssignments(int userId,
        int? offset, int? limit);

    Task<AssignmentBaseModal?> GetAssignmentBaseInfoById(int assignmentId);

    Task<CountedResponse<List<UserAssignmentBaseModal>>?> GetAssigmentUsersTasks(int assignmentId,
        int statusId, int? offset, int? limit);
}