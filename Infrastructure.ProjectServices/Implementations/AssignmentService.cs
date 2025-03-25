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
    public async Task<ResponseViewModel<AssignmentBaseModal>> CreateNewAssignment(
        CreateNewAssignmentsRequest request)
    {
        var res = await assignmentRepository.CreateNewAssignment(request.UserId,
            request.Instructions, request.GroupId, request.EssayId, request.DueDate);
        return new ResponseViewModel<AssignmentBaseModal>()
        {
            Code = res != null ? 0 : -1,
            Message = res != null ? "Assignment created" : "Assignment create failed",
            Data = res
        };
    }

    public async Task<ResponseViewModel<AssignmentBaseModal>> UpdateAssignment(UpdateAssignmentRequest request)
    {
        var res = await assignmentRepository.UpdateAssignment(request.UserId, request.AssignmentId,
            request.Instructions, request.EssayId, request.DueDate);
        return new ResponseViewModel<AssignmentBaseModal>()
        {
            Code = res != null ? 0 : -1,
            Message = res != null ? "Assignment Updated" : "Assignment Update Failed",
            Data = res
        };
    }

    public async Task<ResponseViewModel<CountedResponse<List<AssignmentBaseModal>>>> GetGroupAssignments(int userId, int groupId,int? offset, int? limit)
    {
        var (res, count) = await assignmentRepository.GetGroupAssignments(userId, groupId, offset, limit);
        return new ResponseViewModel<CountedResponse<List<AssignmentBaseModal>>>()
        {
            Code = res != null ? 0 : -1,
            Message = res != null ? "" : "Failed To Get Group Assignments",
            Data = new CountedResponse<List<AssignmentBaseModal>>()
            {
                Result = res,
                TotalCount = count
            }
        };
    }

    public async Task<ResponseViewModel<CountedResponse<List<UserAssignmentBaseModal>>>> GetUserAssignments(int userId, int statusId,int? offset, int? limit)
    {
        var (res, count) = await assignmentRepository.GetUserAssignments(userId, statusId, offset, limit);
        return new ResponseViewModel<CountedResponse<List<UserAssignmentBaseModal>>>()
        {
            Code = res != null ? 0 : -1,
            Message = res != null ? "" : "Failed To Get User Assignments",
            Data = new CountedResponse<List<UserAssignmentBaseModal>>()
            {
                Result = res,
                TotalCount = count
            }
        };
    }

    public async Task<ResponseViewModel<ResultResponse>> SaveOrSubmitAssignment(SaveOrSubmitAssignmentRequest request)
    {
        var res = await assignmentRepository.SaveOrSubmitAssignment(request.UserId, request.AssignmentId, request.Text,
            request.WordCount, request.IsSubmitted);
        return new ResponseViewModel<ResultResponse>()
        {
            Code = res ? 0 : -1,
            Message = res ? "" : "Failed To Save Or Submit Assignment",
            Data = new ResultResponse()
            {
                Result = res
            }
        };
    }

    public async Task<ResponseViewModel<UserAssignmentModal>> GetUserAssignment(int callerId, int userId,
        int assignmentId)
    {
        var res = await assignmentRepository.GetUserAssignment(callerId, userId, assignmentId);
        return new ResponseViewModel<UserAssignmentModal>()
        {
            Code = res != null ? 0 : -1,
            Message = res != null ? "" : "FaiLed To Get User Assignment",
            Data = res
        };
    }

    public async Task<ResponseViewModel<CountedResponse<List<UserAssignmentBaseModal>>>> GetUserNotSeenEvaluatedAssignments(int userId,int? offset, int? limit)
    {
        var (res, count) = await assignmentRepository.GetUserNotSeenEvaluatedAssignments(userId, offset, limit);
        return new ResponseViewModel<CountedResponse<List<UserAssignmentBaseModal>>>()
        {
            Code = res != null ? 0 : -1,
            Message = res != null ? "" : "Failed To Get UserNotSeenEvaluatedAssignments",
            Data = new CountedResponse<List<UserAssignmentBaseModal>>()
            {
                Result = res,
                TotalCount = count
            }
        };
    }

    public async Task<ResponseViewModel<List<StatusBaseModal>>> GetEvaluationTextStatuses()
    {
        var res = await assignmentRepository.GetEvaluationStatuses();
        return new ResponseViewModel<List<StatusBaseModal>>()
        {
            Code = res != null ? 0 : -1,
            Message = res != null ? "" : "FaiLed To Get EvaluationTextStatuses",
            Data = res
        };
    }

    public async Task<ResponseViewModel<List<StatusBaseModal>>> GetAssignmentStatuses()
    {
        var res = await assignmentRepository.GetAssignmentStatuses();
        return new ResponseViewModel<List<StatusBaseModal>>()
        {
            Code = res != null ? 0 : -1,
            Message = res != null ? "" : "FaiLed To Get AssignmentStatuses",
            Data = res
        };
    }

    public async Task<ResponseViewModel<ResultResponse>> EvaluateAssignments(EvaluateAssignmentRequest request)
    {
        var res = await assignmentRepository.EvaluateAssignment(request.TeacherId, request.UserId, request.AssignmentId,
            request.FluencyScore, request.GrammarScore, request.EvaluationTextComments, request.GeneralComments);
        return new ResponseViewModel<ResultResponse>()
        {
            Code = res ? 0 : 1,
            Message = res ? "" : "Failed To Evaluate Assignment",
            Data = new ResultResponse() { Result = res }
        };
    }

    public async Task<ResponseViewModel<CountedResponse<List<UserAssignmentBaseModal>>>> GetTeacherAssignments(int userId,int? offset, int? limit)
    {
        var (res, count) = await assignmentRepository.GetTeacherAssignments(userId, offset, limit);
        return new ResponseViewModel<CountedResponse<List<UserAssignmentBaseModal>>>()
        {
            Code = res != null ? 0 : -1,
            Message = res != null ? "" : "Failed To Get TeacherAssignments",
            Data = new CountedResponse<List<UserAssignmentBaseModal>>()
            {
                Result = res,
                TotalCount = count
            }
        };
    }

    public async Task<ResponseViewModel<AssignmentBaseModal>> GetAssignmentBaseInfoById(int assignmentId)
    {
        var res = await assignmentRepository.GetAssignmentById(assignmentId);
        return new ResponseViewModel<AssignmentBaseModal>()
        {
            Code = res != null ? 0 : -1,
            Message = res != null ? "" : "Failed To Get Assignment Base Info",
            Data = res
        };
    }

    public async Task<ResponseViewModel<CountedResponse<List<UserAssignmentBaseModal>>>> GetAssigmentUsersTasks(int assignmentId,
        int statusId, int? offset, int? limit)
    {
        var (res, count) = await assignmentRepository.GetAssigmentUsersTasks(assignmentId, statusId, offset, limit);
        return new ResponseViewModel<CountedResponse<List<UserAssignmentBaseModal>>>()
        {
            Code = res != null ? 0 : -1,
            Message = res != null ? "" : "Failed To Get Assignment User Tasks",
            Data = new CountedResponse<List<UserAssignmentBaseModal>>()
            {
                Result = res,
                TotalCount = count
            }
        };
    }
}