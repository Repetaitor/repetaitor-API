using Core.Application.Models;
using Core.Application.Models.DTO;
using Core.Application.Models.DTO.Assignments;
using Core.Application.Models.DTO.Essays;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Application.Interfaces.Services;

public interface IAssignmentService
{
    Task<ResponseView<AssignmentBaseModal>> CreateNewAssignment(int userId, CreateNewAssignmentsRequest request);

    Task<ResponseView<AssignmentBaseModal>> UpdateAssignment(int userId, UpdateAssignmentRequest request);

    Task<ResponseView<CountedResponse<List<AssignmentBaseModal>>>> GetGroupAssignments(int userId, int groupId,
        int? offset, int? limit);

    Task<ResponseView<CountedResponse<List<UserAssignmentBaseModal>>>> GetUserAssignments(int userId, string statusName,
        bool isAIAssignment, int? offset, int? limit);

    Task<ResponseView<ResultResponse>> SaveOrSubmitAssignment(int userId, SaveOrSubmitAssignmentRequest request);
    Task<ResponseView<UserAssignmentModal>> GetUserAssignment(int callerId, int userId, int assignmentId);

    Task<ResponseView<CountedResponse<List<UserAssignmentBaseModal>>>> GetUserNotSeenEvaluatedAssignments(
        int userId, int? offset, int? limit);

    Task<ResponseView<List<StatusBaseModal>>> GetEvaluationTextStatuses();

    Task<ResponseView<List<StatusBaseModal>>> GetAssignmentStatuses();

    Task<ResponseView<ResultResponse>> EvaluateAssignments(int teacherId, EvaluateAssignmentRequest request);

    Task<ResponseView<CountedResponse<List<UserAssignmentBaseModal>>>> GetTeacherAssignments(int userId,
        int? offset, int? limit);

    Task<ResponseView<AssignmentBaseModal>> GetAssignmentBaseInfoById(int assignmentId);

    Task<ResponseView<CountedResponse<List<UserAssignmentBaseModal>>>> GetAssigmentUsersTasks(int userId, int assignmentId,
        string statusName, int? offset, int? limit);
    Task<ResponseView<List<UserAssignmentViewForAI>>> GetUserAssignmentViewForAI(int aiTeacherId, int count);
}