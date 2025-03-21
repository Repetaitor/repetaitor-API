using Azure.Core;
using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.DTO.Assignments;
using Core.Application.Models.DTO.Essays;

namespace Infrastructure.ProjectServices.Implementations;

public class AssignmentService(IAssignmentRepository assignmentRepository) : IAssignmentService
{
    public async Task<ResponseViewModel<BaseAssignmentResponse>> CreateNewAssignment(
        CreateNewAssignmentsRequest request)
    {
        var res = await assignmentRepository.CreateNewAssignment(request.UserId, request.AssignmentTitle,
            request.Instructions, request.GroupId, request.EssayId, request.DueDate);
        return new ResponseViewModel<BaseAssignmentResponse>()
        {
            Code = res != null ? 0 : -1,
            Message = res != null ? "Assignment created" : "Assignment create failed",
            Data = res != null
                ? new BaseAssignmentResponse()
                {
                    AssignmentId = res.Id,
                    AssignmentName = res.AssignmentTitle,
                    Instructions = res.Instructions,
                    Creator = res.Creator,
                    DueDate = res.DueDate,
                    ExpectedWordCount = res.Essay?.ExpectedWordCount ?? 0
                }
                : null
        };
    }

    public async Task<ResponseViewModel<BaseAssignmentResponse>> UpdateAssignment(UpdateAssignmentRequest request)
    {
        var res = await assignmentRepository.UpdateAssignment(request.UserId, request.AssignmentId,
            request.AssignmentTitle, request.Instructions, request.EssayId);
        return new ResponseViewModel<BaseAssignmentResponse>()
        {
            Code = res != null ? 0 : -1,
            Message = res != null ? "Assignment Updated" : "Assignment Update Failed",
            Data = res != null
                ? new BaseAssignmentResponse()
                {
                    AssignmentId = res.Id,
                    AssignmentName = res.AssignmentTitle,
                    Instructions = res.Instructions,
                    Creator = res.Creator,
                    DueDate = res.DueDate,
                    ExpectedWordCount = res.Essay?.ExpectedWordCount ?? 0
                }
                : null
        };
    }

    public async Task<ResponseViewModel<List<BaseAssignmentResponse>>> GetGroupAssignments(int userId, int groupId)
    {
        var res = await assignmentRepository.GetGroupAssignments(userId, groupId);
        return new ResponseViewModel<List<BaseAssignmentResponse>>()
        {
            Code = res != null ? 0 : -1,
            Message = res != null ? "" : "Failed To Get Group Assignments",
            Data = res != null
                ? res.Select(x => new BaseAssignmentResponse()
                {
                    AssignmentId = x.Id,
                    AssignmentName = x.AssignmentTitle,
                    Instructions = x.Instructions,
                    Creator = x.Creator,
                    DueDate = x.DueDate,
                    ExpectedWordCount = x.Essay?.ExpectedWordCount ?? 0
                }).ToList()
                : []
        };
    }

    public async Task<ResponseViewModel<List<BaseAssignmentResponse>>> GetUserAssignments(int userId, int statusId)
    {
        var res = await assignmentRepository.GetUserAssignments(userId, statusId);
        return new ResponseViewModel<List<BaseAssignmentResponse>>()
        {
            Code = res != null ? 0 : -1,
            Message = res != null ? "" : "Failed To Get User Assignments",
            Data = res != null
                ? res.Select(x => new BaseAssignmentResponse()
                {
                    AssignmentId = x.Id,
                    AssignmentName = x.AssignmentTitle ?? "",
                    Instructions = x.Instructions ?? "",
                    Status = x.Status ?? null,
                    Creator = x.Creator,
                    DueDate = x.DueDate,
                    ExpectedWordCount = x.Essay?.ExpectedWordCount ?? 0,
                    TotalScore = x.TotalScore
                }).ToList()
                : []
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

    public async Task<ResponseViewModel<List<BaseAssignmentResponse>>> GetUserNotSeenEvaluatedAssignments(int userId)
    {
        var res = await assignmentRepository.GetUserNotSeenEvaluatedAssignments(userId);
        return new ResponseViewModel<List<BaseAssignmentResponse>>()
        {
            Code = res != null ? 0 : -1,
            Message = res != null ? "" : "Failed To Get UserNotSeenEvaluatedAssignments",
            Data = res != null
                ? res.Select(x => new BaseAssignmentResponse()
                {
                    AssignmentId = x.Id,
                    AssignmentName = x.AssignmentTitle ?? "",
                    Instructions = x.Instructions ?? "",
                    Creator = x.Creator,
                    DueDate = x.DueDate,
                    Status = x.Status ?? null,
                    ExpectedWordCount = x.Essay?.ExpectedWordCount ?? 0,
                    TotalScore = x.TotalScore
                }).ToList()
                : []
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

    public async Task<ResponseViewModel<List<BaseAssignmentResponse>>> GetTeacherAssignments(int userId)
    {
        var res = await assignmentRepository.GetTeacherAssignments(userId);
        return new ResponseViewModel<List<BaseAssignmentResponse>>()
        {
            Code = res != null ? 0 : -1,
            Message = res != null ? "" : "Failed To Get TeacherAssignments",
            Data = res != null
                ? res.Select(x => new BaseAssignmentResponse()
                {
                    AssignmentId = x.Id,
                    AssignmentName = x.AssignmentTitle ?? "",
                    Instructions = x.Instructions ?? "",
                    Creator = x.Creator,
                    DueDate = x.DueDate,
                    Status = x.Status ?? null,
                    ExpectedWordCount = x.Essay?.ExpectedWordCount ?? 0,
                    TotalScore = x.TotalScore
                }).ToList()
                : []
        };
    }
}