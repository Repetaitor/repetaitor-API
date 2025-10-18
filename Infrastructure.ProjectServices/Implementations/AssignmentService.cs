using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.QuizModels;
using Core.Application.Models.RequestsDTO;
using Core.Application.Models.RequestsDTO.Assignments;
using Core.Application.Models.RequestsDTO.Essays;
using Core.Application.Models.ReturnViewModels;
using Core.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ProjectServices.Implementations;

public class AssignmentService(
    IAssignmentRepository assignmentRepository,
    IAICommunicateService aiCommunicateService,
    ILogger<AssignmentService> logger) : IAssignmentService
{
    public async Task<ResponseView<QuizViewModel>> CreateUserQuizAsync(List<string> questionTypes)
    {
        try
        {
            var quiz = await aiCommunicateService.GetQuizQuestions(questionTypes);
            return new ResponseView<QuizViewModel>()
            {
                Code = StatusCodesEnum.Success,
                Data = quiz
            };
        }
        catch (Exception ex)
        {
            return new ResponseView<QuizViewModel>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "",
                Data = null
            };
        }
    }
    public async Task<string> GetEssayTitle()
    {
        try
        {
            return await aiCommunicateService.GetEssayTitle();
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    public async Task<bool> AddDetailedScores(int userId, int assignmentId, int vocabulary, int spellingAndPunctuation, int grammar)
    {
        try
        {
            return await assignmentRepository.AddDetailedScores(userId, assignmentId, vocabulary, spellingAndPunctuation, grammar);
        }
        catch (Exception ex)
        {
            logger.LogInformation("DeleteAssignment exception: {ex}", ex.Message);
            return false;
        }
    }

    public async Task<ResponseView<string>> GetSuggestion(int userId)
    {
        try
        {
            var res = await assignmentRepository.GetSuggestion(userId);
            return new ResponseView<string>()
            {
                Code = StatusCodesEnum.Success,
                Data = res.SuggetionText
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("DeleteAssignment exception: {ex}", ex.Message);
            return new ResponseView<string>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "An error occurred while deleting the assignment: " + ex.Message,
                Data = ""
            };
        }
    }

    public async Task<ResponseView<ResultResponse>> DeleteAssignment(int userId, int assignmentId)
    {
        try
        {
            var res = await assignmentRepository.DeleteAssignment(userId, assignmentId);
            return new ResponseView<ResultResponse>()
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("DeleteAssignment exception: {ex}", ex.Message);
            return new ResponseView<ResultResponse>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "An error occurred while deleting the assignment: " + ex.Message,
                Data = new ResultResponse() { Result = false }
            };
        }
    }

    public async Task<ResponseView<AssignmentBaseModal>> CreateNewAssignment(
        int userId, CreateNewAssignmentsRequest request)
    {
        try
        {
            var res = await assignmentRepository.CreateNewAssignment(userId,
                request.Instructions, request.GroupId, request.EssayId, request.DueDate);
            return new ResponseView<AssignmentBaseModal>()
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("CreateNewAssignment exception: {ex}", ex.Message);
            return new ResponseView<AssignmentBaseModal>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "An error occurred while creating a new assignment: " + ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<AssignmentBaseModal>> UpdateAssignment(int userId, UpdateAssignmentRequest request)
    {
        try
        {
            var res = await assignmentRepository.UpdateAssignment(userId, request.AssignmentId,
                request.Instructions, request.EssayId, request.DueDate);
            return new ResponseView<AssignmentBaseModal>()
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("UpdateAssignment exception: {ex}", ex.Message);
            return new ResponseView<AssignmentBaseModal>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "An error occurred while updating the assignment: " + ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<PaginatedResponse<List<GroupAssignmentBaseModal>>>> GetGroupAssignments(int userId,
        int groupId, int? pageIndex, int? pageSize)
    {
        try
        {
            var res = await assignmentRepository.GetGroupAssignments(userId, groupId, pageIndex, pageSize);
            return new ResponseView<PaginatedResponse<List<GroupAssignmentBaseModal>>>()
            {
                Code = StatusCodesEnum.Success,
                Data = new PaginatedResponse<List<GroupAssignmentBaseModal>>(
                    res.Item2,
                    pageSize ?? 5,
                    pageIndex ?? 1,
                    res.Item1 ?? []
                )
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("GetGroupAssignments exception: {ex}", ex.Message);
            return new ResponseView<PaginatedResponse<List<GroupAssignmentBaseModal>>>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "An error occurred while fetching group assignments: " + ex.Message,
                Data = new PaginatedResponse<List<GroupAssignmentBaseModal>>(
                    0,
                    pageSize ?? 5,
                    pageIndex ?? 1,
                    null!
                )
            };
        }
    }

    public async Task<ResponseView<PaginatedResponse<List<UserAssignmentBaseModal>>>> GetUserAssignments(int userId,
        string statusName, bool isAIAssignment, int? pageIndex, int? pageSize)
    {
        try
        {
            var res = await assignmentRepository.GetUserAssignments(userId, statusName, isAIAssignment, pageIndex,
                pageSize);
            return new ResponseView<PaginatedResponse<List<UserAssignmentBaseModal>>>()
            {
                Code = StatusCodesEnum.Success,
                Data = new PaginatedResponse<List<UserAssignmentBaseModal>>(
                    res.Item2,
                    pageSize ?? 5,
                    pageIndex ?? 1,
                    res.Item1 ?? []
                )
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("GetUserAssignments exception: {ex}", ex.Message);
            return new ResponseView<PaginatedResponse<List<UserAssignmentBaseModal>>>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "An error occurred while fetching user assignments: " + ex.Message,
                Data = new PaginatedResponse<List<UserAssignmentBaseModal>>(
                    0,
                    pageSize ?? 5,
                    pageIndex ?? 1,
                    null)
            };
        }
    }

    public async Task<ResponseView<ResultResponse>> SaveOrSubmitAssignment(int userId,
        SaveOrSubmitAssignmentRequest request)
    {
        try
        {
            if (request is { Text: "", Images.Count: > 0 })
            {
                var rs = await aiCommunicateService.GetEssayTextFromImage(request.Images);
                request.Text = rs;
            }

            var res = await assignmentRepository.SaveOrSubmitAssignment(userId, request.AssignmentId,
                request.Text ?? "",
                request.WordCount, request.IsSubmitted);

            var clear1 = await assignmentRepository.ClearUserAssignemntImagesFromDb(userId, request.AssignmentId);
            if (!clear1)
            {
                return new ResponseView<ResultResponse>()
                {
                    Code = StatusCodesEnum.InternalServerError,
                    Message = "Failed to clear previous images.",
                    Data = new ResultResponse() { Result = false }
                };
            }

            if (res && request.Images.Count > 0)
            {
                await assignmentRepository.SaveImagesForAssignmentDb(userId, request.AssignmentId, request.Images);
            }

            return new ResponseView<ResultResponse>()
            {
                Code = StatusCodesEnum.Success,
                Data = new ResultResponse() { Result = res }
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("AssignmentService exception: {ex}", ex.Message);
            return new ResponseView<ResultResponse>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "An error occurred while saving or submitting the assignment: " + ex.Message,
                Data = new ResultResponse() { Result = false }
            };
        }
    }

    public async Task<ResponseView<UserAssignmentModal>> GetUserAssignment(int callerId, int userId,
        int assignmentId)
    {
        try
        {
            var res = await assignmentRepository.GetUserAssignment(callerId, userId, assignmentId);
            res.Images = await assignmentRepository.GetImagesForUserAssignmentDb(userId, assignmentId);
            return new ResponseView<UserAssignmentModal>()
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("GetUserAssignment exception: {ex}", ex.Message);
            return new ResponseView<UserAssignmentModal>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "An error occurred while fetching the user assignment: " + ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<PaginatedResponse<List<UserAssignmentBaseModal>>>> GetUserNotSeenEvaluatedAssignments(
        int userId, int? pageIndex, int? pageSize)
    {
        try
        {
            var res = await assignmentRepository.GetUserNotSeenEvaluatedAssignments(userId, pageIndex, pageSize);
            return new ResponseView<PaginatedResponse<List<UserAssignmentBaseModal>>>()
            {
                Code = StatusCodesEnum.Success,
                Data = new PaginatedResponse<List<UserAssignmentBaseModal>>(
                    res.Item2,
                    pageSize ?? 5,
                    pageIndex ?? 1,
                    res.Item1 ?? []
                )
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("GetUserNotSeenEvaluatedAssignments exception: {ex}", ex.Message);
            return new ResponseView<PaginatedResponse<List<UserAssignmentBaseModal>>>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "An error occurred while fetching user not seen evaluated assignments: " + ex.Message,
                Data = new PaginatedResponse<List<UserAssignmentBaseModal>>(
                    0,
                    pageSize ?? 5,
                    pageIndex ?? 1,
                    null)
            };
        }
    }

    public async Task<ResponseView<List<StatusBaseModal>>> GetEvaluationTextStatuses()
    {
        try
        {
            var res = await assignmentRepository.GetEvaluationStatuses();
            return new ResponseView<List<StatusBaseModal>>
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("GetEvaluationStatuses exception: {ex}", ex.Message);
            return new ResponseView<List<StatusBaseModal>>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "An error occurred while fetching evaluation statuses: " + ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<List<StatusBaseModal>>> GetAssignmentStatuses()
    {
        try
        {
            var res = await assignmentRepository.GetAssignmentStatuses();
            return new ResponseView<List<StatusBaseModal>>()
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("GetAssignmentStatuses exception: {ex}", ex.Message);
            return new ResponseView<List<StatusBaseModal>>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "An error occurred while fetching assignment statuses: " + ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<ResultResponse>> EvaluateAssignments(int teacherId,
        EvaluateAssignmentRequest request)
    {
        try
        {
            var res = await assignmentRepository.EvaluateAssignment(teacherId, request.UserId, request.AssignmentId,
                request.FluencyScore, request.GrammarScore, request.EvaluationTextComments, request.GeneralComments);
            return new ResponseView<ResultResponse>()
            {
                Code = StatusCodesEnum.Success,
                Data = new ResultResponse() { Result = res }
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("EvaluateAssignment exception: {ex}", ex.Message);
            return new ResponseView<ResultResponse>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "An error occurred while evaluating the assignment: " + ex.Message,
                Data = new ResultResponse() { Result = false }
            };
        }
    }

    public async Task<ResponseView<PaginatedResponse<List<UserAssignmentBaseModal>>>> GetTeacherAssignments(
        int userId, int? pageIndex, int? pageSize)
    {
        try
        {
            var res = await assignmentRepository.GetTeacherAssignments(userId, pageIndex, pageSize);
            return new ResponseView<PaginatedResponse<List<UserAssignmentBaseModal>>>()
            {
                Code = StatusCodesEnum.Success,
                Data = new PaginatedResponse<List<UserAssignmentBaseModal>>(
                    res.Item2,
                    pageSize ?? 5,
                    pageIndex ?? 1,
                    res.Item1 ?? []
                )
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("GetTeacherAssignments exception: {ex}", ex.Message);
            return new ResponseView<PaginatedResponse<List<UserAssignmentBaseModal>>>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "An error occurred while fetching teacher assignments: " + ex.Message,
                Data = new PaginatedResponse<List<UserAssignmentBaseModal>>(
                    0,
                    pageSize ?? 5,
                    pageIndex ?? 1,
                    null)
            };
        }
    }

    public async Task<ResponseView<AssignmentBaseModal>> GetAssignmentBaseInfoById(int assignmentId)
    {
        try
        {
            var res = await assignmentRepository.GetAssignmentById(assignmentId);
            return new ResponseView<AssignmentBaseModal>()
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("GetAssignmentById exception: {ex}", ex.Message);
            return new ResponseView<AssignmentBaseModal>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "An error occurred while fetching assignment info: " + ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<PaginatedResponse<List<UserAssignmentBaseModal>>>> GetAssigmentUsersTasks(int userId,
        int assignmentId,
        string statusName, int? pageIndex, int? pageSize)
    {
        try
        {
            var res = await assignmentRepository.GetAssigmentUsersTasks(userId, assignmentId, statusName, pageIndex,
                pageSize);
            return new ResponseView<PaginatedResponse<List<UserAssignmentBaseModal>>>()
            {
                Code = StatusCodesEnum.Success,
                Data = new PaginatedResponse<List<UserAssignmentBaseModal>>(
                    res.Item2,
                    pageSize ?? 5,
                    pageIndex ?? 1,
                    res.Item1 ?? []
                )
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("GetAssigmentUsersTasks exception: {ex}", ex.Message);
            return new ResponseView<PaginatedResponse<List<UserAssignmentBaseModal>>>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "An error occurred while fetching assignment users tasks: " + ex.Message,
                Data = new PaginatedResponse<List<UserAssignmentBaseModal>>(
                    0,
                    pageSize ?? 5,
                    pageIndex ?? 1,
                    null)
            };
        }
    }

    public async Task<ResponseView<List<UserAssignmentViewForAI>>> GetUserAssignmentViewForAI(int aiTeacherId,
        int count)
    {
        try
        {
            var res = await assignmentRepository.GetUserAssignmentViewForAI(aiTeacherId, count);
            return new ResponseView<List<UserAssignmentViewForAI>>()
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("GetUserAssignmentViewForAI exception: {ex}", ex.Message);
            return new ResponseView<List<UserAssignmentViewForAI>>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "An error occurred while fetching user assignment view for AI: " + ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<List<string>>> GetUserAssignmentImages(int userId, int assignmentId)
    {
        try
        {
            var images = await assignmentRepository.GetUserAssignmentImagesUrl(userId, assignmentId);
            return new ResponseView<List<string>>()
            {
                Code = StatusCodesEnum.Success,
                Data = images
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("GetUserAssignmentImagesUrl exception: {ex}", ex.Message);
            return new ResponseView<List<string>>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "An error occurred while fetching user assignment images: " + ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<ResultResponse>> MakeUserAssignmentPublic(int userId, int assignmentId)
    {
        try
        {
            var res = await assignmentRepository.MakeUserAssignmentPublic(userId, assignmentId);
            return new ResponseView<ResultResponse>()
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("MakeUserAssignmentPublic exception: {ex}", ex.Message);
            return new ResponseView<ResultResponse>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "An error occurred while making user assignment public: " + ex.Message,
                Data = new ResultResponse() { Result = false }
            };
        }
    }

    public async Task<ResponseView<List<UserAssignmentBaseModal>>> GetPublicUserAssignments(int userId,
        int assignmentId, int? offset, int? limit)
    {
        try
        {
            var userAssignments =
                await assignmentRepository.GetPublicUserAssignments(userId, assignmentId, offset, limit);
            return new ResponseView<List<UserAssignmentBaseModal>>()
            {
                Code = StatusCodesEnum.Success,
                Data = userAssignments
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("GetPublicUserAssignments exception: {ex}", ex.Message);
            return new ResponseView<List<UserAssignmentBaseModal>>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "An error occurred while fetching public user assignments: " + ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<ResultResponse>> IsAssignmentPublic(int userId, int assignmentId)
    {
        try
        {
            var isPublicResult = await assignmentRepository.IsAssignmentPublic(userId, assignmentId);
            return new ResponseView<ResultResponse>()
            {
                Code = StatusCodesEnum.Success,
                Data = isPublicResult
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("IsAssignmentPublic exception: {ex}", ex.Message);
            return new ResponseView<ResultResponse>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "An error occurred while checking if assignment is public: " + ex.Message,
                Data = new ResultResponse() { Result = false }
            };
        }
    }
}