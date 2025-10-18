using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.Models;
using Core.Application.Models.QuizModels;
using Core.Application.Models.ReturnViewModels;
using Core.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ProjectServices.Implementations;

public class UserService(
    IAICommunicateService aiCommunicateService,
    IUserRepository userRepository,
    IAssignmentRepository assignmentRepository,
    IGroupRepository groupRepository,
    IEssayRepository essayRepository,
    ILogger<AssignmentService> logger) : IUserService
{
    public async Task<ResponseView<UserModal>> GetUserDefaultInfoAsync(int userId)
    {
        try
        {
            var res = await userRepository.GetUserInfo(userId);
            return new ResponseView<UserModal>
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("GetUserInfo exception: {ex}", ex.Message);
            return new ResponseView<UserModal>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<StudentDashboardHeaderInfoViewModel>> GetStudentDashboardHeaderInfoAsync(int userId)
    {
        try
        {
            var userStats = await assignmentRepository.GetAverageUserScoreByDate(userId);
            var userAssignmentsStats = await assignmentRepository.GetUserAssignmentsStatusStat(userId);
            var userPerformance = await assignmentRepository.GetUserPerformance(userId);
            return new ResponseView<StudentDashboardHeaderInfoViewModel>
            {
                Code = StatusCodesEnum.Success,
                Data = new StudentDashboardHeaderInfoViewModel
                {
                    CompletedAssignmentsCount = userAssignmentsStats.CompletedAssignmentsCount,
                    InProgressAssignmentsCount = userAssignmentsStats.InProgressAssignmentsCount,
                    PendingAssignmentsCount = userAssignmentsStats.PendingAssignmentsCount,
                    UserScoresStats = userStats,
                    UserPerformanceStats = userPerformance.PerformanceStats
                }
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("GetStudentDashboardHeaderInfoAsync exception: {ex}", ex.Message);
            return new ResponseView<StudentDashboardHeaderInfoViewModel>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<UserPerformanceViewModel>> GetUserPerformanceAsync(int userId,
        DateTime? fromDate = null, DateTime? toDate = null)
    {
        try
        {
            var res = await assignmentRepository.GetUserPerformance(userId, fromDate, toDate);
            return new ResponseView<UserPerformanceViewModel>()
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("GetUserPerformance exception: {ex}", ex.Message);
            return new ResponseView<UserPerformanceViewModel>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<TeacherDashboardBaseInfo>> GetTeacherDashboardHeaderInfoAsync(int teacherId)
    {
        try
        {
            var groupsCount = await groupRepository.TeacherGroupsCount(teacherId);
            var enrolledStudentsCount = await groupRepository.TeacherGroupsEnrolledStudentsCount(teacherId);
            var createdAssignmentsCount = await assignmentRepository.GetTeacherCreatedAssignmentsCount(teacherId);
            var needToEvaluateAssignmentsCount =
                await assignmentRepository.GetTeacherNeedToEvaluateAssignmentsCount(teacherId);
            var usersPerformanse = await assignmentRepository.GetAllUserPerformanceForTeacher(teacherId);
            var essayCount = await essayRepository.GetEssayCount(teacherId);
            return new ResponseView<TeacherDashboardBaseInfo>
            {
                Code = StatusCodesEnum.Success,
                Data = new TeacherDashboardBaseInfo
                {
                    GroupsCount = groupsCount,
                    EnrolledStudentsCount = enrolledStudentsCount,
                    AssignmentsCount = createdAssignmentsCount,
                    NeedEvaluateAssignmentsCount = needToEvaluateAssignmentsCount,
                    EssayCount = essayCount,
                    GroupPerformanceStats = usersPerformanse.PerformanceStats
                }
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("GetTeacherDashboardHeaderInfoAsync exception: {ex}", ex.Message);
            return new ResponseView<TeacherDashboardBaseInfo>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }

    public async Task<ResponseView<GroupsPerformance>> GetTeacherGroupsPerformanceByDate(int teacherId,
        DateTime? fromDate = null, DateTime? toDate = null)
    {
        try
        {
            var res = await assignmentRepository.GetTeacherGroupsPerformanceByDate(teacherId, fromDate, toDate);
            return new ResponseView<GroupsPerformance>
            {
                Code = StatusCodesEnum.Success,
                Data = res
            };
        }
        catch (Exception ex)
        {
            logger.LogInformation("GetTeacherGroupsPerformanceByDate exception: {ex}", ex.Message);
            return new ResponseView<GroupsPerformance>
            {
                Code = StatusCodesEnum.InternalServerError,
                Message = ex.Message,
                Data = null
            };
        }
    }
}