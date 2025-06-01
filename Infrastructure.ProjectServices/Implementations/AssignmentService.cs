using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.DTO;
using Core.Application.Models.DTO.Assignments;
using Core.Application.Models.DTO.Essays;

namespace Infrastructure.ProjectServices.Implementations;

public class AssignmentService(IAssignmentRepository assignmentRepository) : IAssignmentService
{
    public async Task<ResponseView<AssignmentBaseModal>> CreateNewAssignment(
        int userId, CreateNewAssignmentsRequest request)
    {
        return await assignmentRepository.CreateNewAssignment(userId,
            request.Instructions, request.GroupId, request.EssayId, request.DueDate);
    }

    public async Task<ResponseView<AssignmentBaseModal>> UpdateAssignment(int userId, UpdateAssignmentRequest request)
    {
        return await assignmentRepository.UpdateAssignment(userId, request.AssignmentId,
            request.Instructions, request.EssayId, request.DueDate);
    }

    public async Task<ResponseView<CountedResponse<List<AssignmentBaseModal>>>> GetGroupAssignments(int userId,
        int groupId, int? offset, int? limit)
    {
        var res = await assignmentRepository.GetGroupAssignments(userId, groupId, offset, limit);
        return new ResponseView<CountedResponse<List<AssignmentBaseModal>>>()
        {
            Code = res.Code,
            Message = res.Message,
            Data = new CountedResponse<List<AssignmentBaseModal>>()
            {
                Result = res.Data.Item1,
                TotalCount = res.Data.Item2
            }
        };
    }

    public async Task<ResponseView<CountedResponse<List<UserAssignmentBaseModal>>>> GetUserAssignments(int userId,
        string statusName, bool isAIAssignment, int? offset, int? limit)
    {
        var res = await assignmentRepository.GetUserAssignments(userId, statusName, isAIAssignment, offset, limit);
        return new ResponseView<CountedResponse<List<UserAssignmentBaseModal>>>()
        {
            Code = res.Code,
            Message = res.Message,
            Data = new CountedResponse<List<UserAssignmentBaseModal>>()
            {
                Result = res.Data.Item1,
                TotalCount = res.Data.Item2
            }
        };
    }

    public async Task<ResponseView<ResultResponse>> SaveOrSubmitAssignment(int userId,
        SaveOrSubmitAssignmentRequest request)
    {
        var res = await assignmentRepository.SaveOrSubmitAssignment(userId, request.AssignmentId, request.Text,
            request.WordCount, request.IsSubmitted);
        return new ResponseView<ResultResponse>()
        {
            Code = res.Code,
            Message = res.Message,
            Data = new ResultResponse() { Result = res.Data }
        };
    }

    public async Task<ResponseView<UserAssignmentModal>> GetUserAssignment(int callerId, int userId,
        int assignmentId)
    {
        return await assignmentRepository.GetUserAssignment(callerId, userId, assignmentId);
    }

    public async Task<ResponseView<CountedResponse<List<UserAssignmentBaseModal>>>> GetUserNotSeenEvaluatedAssignments(
        int userId, int? offset, int? limit)
    {
        var res = await assignmentRepository.GetUserNotSeenEvaluatedAssignments(userId, offset, limit);
        return new ResponseView<CountedResponse<List<UserAssignmentBaseModal>>>()
        {
            Code = res.Code,
            Message = res.Message,
            Data = new CountedResponse<List<UserAssignmentBaseModal>>()
            {
                Result = res.Data.Item1,
                TotalCount = res.Data.Item2
            }
        };
    }

    public async Task<ResponseView<List<StatusBaseModal>>> GetEvaluationTextStatuses()
    {
        return await assignmentRepository.GetEvaluationStatuses();
    }

    public async Task<ResponseView<List<StatusBaseModal>>> GetAssignmentStatuses()
    {
        return await assignmentRepository.GetAssignmentStatuses();
    }

    public async Task<ResponseView<ResultResponse>> EvaluateAssignments(int teacherId,
        EvaluateAssignmentRequest request)
    {
        var res = await assignmentRepository.EvaluateAssignment(teacherId, request.UserId, request.AssignmentId,
            request.FluencyScore, request.GrammarScore, request.EvaluationTextComments, request.GeneralComments);
        return new ResponseView<ResultResponse>()
        {
            Code = res.Code,
            Message = res.Message,
            Data = new ResultResponse() { Result = res.Data }
        };
    }

    public async Task<ResponseView<CountedResponse<List<UserAssignmentBaseModal>>>> GetTeacherAssignments(
        int userId, int? offset, int? limit)
    {
        var res = await assignmentRepository.GetTeacherAssignments(userId, offset, limit);
        return new ResponseView<CountedResponse<List<UserAssignmentBaseModal>>>()
        {
            Code = res.Code,
            Message = res.Message,
            Data = new CountedResponse<List<UserAssignmentBaseModal>>()
            {
                Result = res.Data.Item1,
                TotalCount = res.Data.Item2
            }
        };
    }

    public async Task<ResponseView<AssignmentBaseModal>> GetAssignmentBaseInfoById(int assignmentId)
    {
        return await assignmentRepository.GetAssignmentById(assignmentId);
    }

    public async Task<ResponseView<CountedResponse<List<UserAssignmentBaseModal>>>> GetAssigmentUsersTasks(int userId,
        int assignmentId,
        string statusName, int? offset, int? limit)
    {
        var res = await assignmentRepository.GetAssigmentUsersTasks(userId, assignmentId, statusName, offset, limit);
        return new ResponseView<CountedResponse<List<UserAssignmentBaseModal>>>()
        {
            Code = res.Code,
            Message = res.Message,
            Data = new CountedResponse<List<UserAssignmentBaseModal>>()
            {
                Result = res.Data.Item1,
                TotalCount = res.Data.Item2
            }
        };
    }

    public async Task<ResponseView<List<UserAssignmentViewForAI>>> GetUserAssignmentViewForAI(int aiTeacherId,
        int count)
    {
        var res = await assignmentRepository.GetUserAssignmentViewForAI(aiTeacherId, count);
        return res;
    }
}