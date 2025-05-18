using Azure.Core;
using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.DTO;
using Core.Application.Models.DTO.Assignments;
using Core.Application.Models.DTO.Essays;

namespace Infrastructure.ProjectServices.Implementations;

public class AssignmentService(IAssignmentRepository assignmentRepository) : IAssignmentService
{
    public async Task<AssignmentBaseModal?> CreateNewAssignment(
        int userId, CreateNewAssignmentsRequest request)
    {
        return await assignmentRepository.CreateNewAssignment(userId,
            request.Instructions, request.GroupId, request.EssayId, request.DueDate);
    }

    public async Task<AssignmentBaseModal?> UpdateAssignment(int userId, UpdateAssignmentRequest request)
    {
        return await assignmentRepository.UpdateAssignment(userId, request.AssignmentId,
            request.Instructions, request.EssayId, request.DueDate);
    }

    public async Task<CountedResponse<List<AssignmentBaseModal>>?> GetGroupAssignments(int userId,
        int groupId, int? offset, int? limit)
    {
        var (res, count) = await assignmentRepository.GetGroupAssignments(userId, groupId, offset, limit);
        return res != null
            ? new CountedResponse<List<AssignmentBaseModal>>()
            {
                Result = res,
                TotalCount = count
            }
            : null;
    }

    public async Task<CountedResponse<List<UserAssignmentBaseModal>>?> GetUserAssignments(int userId,
        string statusName,  bool IsAIAssignment,int? offset, int? limit)
    {
        var (res, count) = await assignmentRepository.GetUserAssignments(userId, statusName, IsAIAssignment, offset, limit);
        return res != null
            ? new CountedResponse<List<UserAssignmentBaseModal>>()
            {
                Result = res,
                TotalCount = count
            }
            : null;
    }

    public async Task<ResultResponse> SaveOrSubmitAssignment(int userId, SaveOrSubmitAssignmentRequest request)
    {
        var res = await assignmentRepository.SaveOrSubmitAssignment(userId, request.AssignmentId, request.Text,
            request.WordCount, request.IsSubmitted);
        return new ResultResponse()
        {
            Result = res
        };
    }

    public async Task<UserAssignmentModal?> GetUserAssignment(int callerId, int userId,
        int assignmentId)
    {
        return await assignmentRepository.GetUserAssignment(callerId, userId, assignmentId);
    }

    public async Task<CountedResponse<List<UserAssignmentBaseModal>>?>
        GetUserNotSeenEvaluatedAssignments(int userId, int? offset, int? limit)
    {
        var (res, count) = await assignmentRepository.GetUserNotSeenEvaluatedAssignments(userId, offset, limit);
        return res != null
            ? new CountedResponse<List<UserAssignmentBaseModal>>()
            {
                Result = res,
                TotalCount = count
            }
            : null;
    }

    public async Task<List<StatusBaseModal>?> GetEvaluationTextStatuses()
    {
        return await assignmentRepository.GetEvaluationStatuses();
    }

    public async Task<List<StatusBaseModal>?> GetAssignmentStatuses()
    {
        return await assignmentRepository.GetAssignmentStatuses();
    }

    public async Task<ResultResponse> EvaluateAssignments(int teacherId, EvaluateAssignmentRequest request)
    {
        var res = await assignmentRepository.EvaluateAssignment(teacherId, request.UserId, request.AssignmentId,
            request.FluencyScore, request.GrammarScore, request.EvaluationTextComments, request.GeneralComments);
        return new ResultResponse() { Result = res };
    }

    public async Task<CountedResponse<List<UserAssignmentBaseModal>>?> GetTeacherAssignments(
        int userId, int? offset, int? limit)
    {
        var (res, count) = await assignmentRepository.GetTeacherAssignments(userId, offset, limit);
        return res != null
            ? new CountedResponse<List<UserAssignmentBaseModal>>()
            {
                Result = res,
                TotalCount = count
            }
            : null;
    }

    public async Task<AssignmentBaseModal?> GetAssignmentBaseInfoById(int assignmentId)
    {
        return await assignmentRepository.GetAssignmentById(assignmentId);
    }

    public async Task<CountedResponse<List<UserAssignmentBaseModal>>?> GetAssigmentUsersTasks(
        int userId,
        int assignmentId,
        string statusName, int? offset, int? limit)
    {
        var (res, count) =
            await assignmentRepository.GetAssigmentUsersTasks(userId, assignmentId, statusName, offset, limit);
        return res != null
            ? new CountedResponse<List<UserAssignmentBaseModal>>()
            {
                Result = res,
                TotalCount = count
            }
            : null;
    }

    public async Task<List<UserAssignmentViewForAI>?> GetUserAssignmentViewForAI(int aiTeacherId, int count)
    {
       var res = await assignmentRepository.GetUserAssignmentViewForAI(aiTeacherId, count);
       return res;
    }
}