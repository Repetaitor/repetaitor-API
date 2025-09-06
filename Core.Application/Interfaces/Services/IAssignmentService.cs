using Core.Application.Models;
using Core.Application.Models.RequestsDTO;
using Core.Application.Models.RequestsDTO.Assignments;
using Core.Application.Models.RequestsDTO.Essays;
using Core.Application.Models.ReturnViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core.Application.Interfaces.Services;

public interface IAssignmentService
{
    Task<ResponseView<ResultResponse>> DeleteAssignment(int userId, int assignmentId);
    Task<ResponseView<AssignmentBaseModal>> CreateNewAssignment(int userId, CreateNewAssignmentsRequest request);

    Task<ResponseView<AssignmentBaseModal>> UpdateAssignment(int userId, UpdateAssignmentRequest request);

    Task<ResponseView<PaginatedResponse<List<GroupAssignmentBaseModal>>>> GetGroupAssignments(int userId, int groupId,
        int? pageIndex, int? pageSize);

    Task<ResponseView<PaginatedResponse<List<UserAssignmentBaseModal>>>> GetUserAssignments(int userId, string statusName,
        bool isAIAssignment, int? pageIndex, int? pageSize);

    Task<ResponseView<ResultResponse>> SaveOrSubmitAssignment(int userId, SaveOrSubmitAssignmentRequest request);
    Task<ResponseView<UserAssignmentModal>> GetUserAssignment(int callerId, int userId, int assignmentId);

    Task<ResponseView<PaginatedResponse<List<UserAssignmentBaseModal>>>> GetUserNotSeenEvaluatedAssignments(
        int userId, int? pageIndex, int? pageSize);

    Task<ResponseView<List<StatusBaseModal>>> GetEvaluationTextStatuses();

    Task<ResponseView<List<StatusBaseModal>>> GetAssignmentStatuses();

    Task<ResponseView<ResultResponse>> EvaluateAssignments(int teacherId, EvaluateAssignmentRequest request);

    Task<ResponseView<PaginatedResponse<List<UserAssignmentBaseModal>>>> GetTeacherAssignments(int userId,
        int? pageIndex, int? pageSize);

    Task<ResponseView<AssignmentBaseModal>> GetAssignmentBaseInfoById(int assignmentId);

    Task<ResponseView<PaginatedResponse<List<UserAssignmentBaseModal>>>> GetAssigmentUsersTasks(int userId, int assignmentId,
        string statusName, int? pageIndex, int? pageSize);
    Task<ResponseView<List<UserAssignmentViewForAI>>> GetUserAssignmentViewForAI(int aiTeacherId, int count);
    Task<ResponseView<List<string>>> GetUserAssignmentImages(int userId, int assignmentId);
    
    Task<ResponseView<ResultResponse>> MakeUserAssignmentPublic(int userId, int assignmentId);
    
    Task<ResponseView<List<UserAssignmentBaseModal>>> GetPublicUserAssignments(int userId, int assignmentId, int? offset, int? limit);
    Task<ResponseView<ResultResponse>> IsAssignmentPublic(int userId, int assignmentId);
}