using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.DTO;
using Core.Application.Models.DTO.Assignments;
using Core.Application.Models.DTO.Essays;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ProjectServices.Implementations;

public class AssignmentService(
    IAssignmentRepository assignmentRepository,
    IAICommunicateService aiCommunicateService,
    ILogger<AssignmentService> logger) : IAssignmentService
{
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

    public async Task<ResponseView<CountedResponse<List<GroupAssignmentBaseModal>>>> GetGroupAssignments(int userId,
        int groupId, int? offset, int? limit)
    {
        try
        {
            var res = await assignmentRepository.GetGroupAssignments(userId, groupId, offset, limit);
            return new ResponseView<CountedResponse<List<GroupAssignmentBaseModal>>>()
            {
                Code = StatusCodesEnum.Success,
                Data = new CountedResponse<List<GroupAssignmentBaseModal>>()
                {
                    Result = res.Item1,
                    TotalCount = res.Item2
                }
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("GetGroupAssignments exception: {ex}", ex.Message);
            return new ResponseView<CountedResponse<List<GroupAssignmentBaseModal>>>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "An error occurred while fetching group assignments: " + ex.Message,
                Data = new CountedResponse<List<GroupAssignmentBaseModal>>()
                {
                    Result = null,
                    TotalCount = 0
                }
            };
        }
    }

    public async Task<ResponseView<CountedResponse<List<UserAssignmentBaseModal>>>> GetUserAssignments(int userId,
        string statusName, bool isAIAssignment, int? offset, int? limit)
    {
        try
        {
            var res = await assignmentRepository.GetUserAssignments(userId, statusName, isAIAssignment, offset, limit);
            return new ResponseView<CountedResponse<List<UserAssignmentBaseModal>>>()
            {
                Code = StatusCodesEnum.Success,
                Data = new CountedResponse<List<UserAssignmentBaseModal>>()
                {
                    Result = res.Item1,
                    TotalCount = res.Item2
                }
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("GetUserAssignments exception: {ex}", ex.Message);
            return new ResponseView<CountedResponse<List<UserAssignmentBaseModal>>>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "An error occurred while fetching user assignments: " + ex.Message,
                Data = new CountedResponse<List<UserAssignmentBaseModal>>()
                {
                    Result = null,
                    TotalCount = 0
                }
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

    public async Task<ResponseView<CountedResponse<List<UserAssignmentBaseModal>>>> GetUserNotSeenEvaluatedAssignments(
        int userId, int? offset, int? limit)
    {
        try
        {
            var res = await assignmentRepository.GetUserNotSeenEvaluatedAssignments(userId, offset, limit);
            return new ResponseView<CountedResponse<List<UserAssignmentBaseModal>>>()
            {
                Code = StatusCodesEnum.Success,
                Data = new CountedResponse<List<UserAssignmentBaseModal>>()
                {
                    Result = res.Item1,
                    TotalCount = res.Item2
                }
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("GetUserNotSeenEvaluatedAssignments exception: {ex}", ex.Message);
            return new ResponseView<CountedResponse<List<UserAssignmentBaseModal>>>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "An error occurred while fetching user not seen evaluated assignments: " + ex.Message,
                Data = new CountedResponse<List<UserAssignmentBaseModal>>()
                {
                    Result = null,
                    TotalCount = 0
                }
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

    public async Task<ResponseView<CountedResponse<List<UserAssignmentBaseModal>>>> GetTeacherAssignments(
        int userId, int? offset, int? limit)
    {
        try
        {
            var res = await assignmentRepository.GetTeacherAssignments(userId, offset, limit);
            return new ResponseView<CountedResponse<List<UserAssignmentBaseModal>>>()
            {
                Code = StatusCodesEnum.Success,
                Data = new CountedResponse<List<UserAssignmentBaseModal>>()
                {
                    Result = res.Item1,
                    TotalCount = res.Item2
                }
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("GetTeacherAssignments exception: {ex}", ex.Message);
            return new ResponseView<CountedResponse<List<UserAssignmentBaseModal>>>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "An error occurred while fetching teacher assignments: " + ex.Message,
                Data = new CountedResponse<List<UserAssignmentBaseModal>>()
                {
                    Result = null,
                    TotalCount = 0
                }
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

    public async Task<ResponseView<CountedResponse<List<UserAssignmentBaseModal>>>> GetAssigmentUsersTasks(int userId,
        int assignmentId,
        string statusName, int? offset, int? limit)
    {
        try
        {
            var res = await assignmentRepository.GetAssigmentUsersTasks(userId, assignmentId, statusName, offset,
                limit);
            return new ResponseView<CountedResponse<List<UserAssignmentBaseModal>>>()
            {
                Code = StatusCodesEnum.Success,
                Data = new CountedResponse<List<UserAssignmentBaseModal>>()
                {
                    Result = res.Item1,
                    TotalCount = res.Item2
                }
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("GetAssigmentUsersTasks exception: {ex}", ex.Message);
            return new ResponseView<CountedResponse<List<UserAssignmentBaseModal>>>()
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = "An error occurred while fetching assignment users tasks: " + ex.Message,
                Data = new CountedResponse<List<UserAssignmentBaseModal>>()
                {
                    Result = null,
                    TotalCount = 0
                }
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