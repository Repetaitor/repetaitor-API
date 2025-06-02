using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.Models;

namespace Infrastructure.ProjectServices.Implementations;

public class UserService(
    IUserRepository userRepository,
    IAssignmentRepository assignmentRepository,
    IGroupRepository groupRepository) : IUserService
{
    public async Task<ResponseView<UserModal>> GetUserDefaultInfoAsync(int userId)
    {
        return await Task.Run(() => userRepository.GetUserInfo(userId));
    }

    public async Task<ResponseView<StudentDashboardHeaderInfoViewModel>> GetStudentDashboardHeaderInfoAsync(int userId)
    {
        var userStats = await assignmentRepository.GetAverageUserScoreByDate(userId);
        if (userStats.Code != StatusCodesEnum.Success || userStats.Data == null)
        {
            return new ResponseView<StudentDashboardHeaderInfoViewModel>
            {
                Code = userStats.Code,
                Message = userStats.Message,
                Data = null
            };
        }

        var userAssignmentsStats = await assignmentRepository.GetUserAssignmentsStatusStat(userId);
        if (userAssignmentsStats.Code != StatusCodesEnum.Success || userAssignmentsStats.Data == null)
        {
            return new ResponseView<StudentDashboardHeaderInfoViewModel>
            {
                Code = userAssignmentsStats.Code,
                Message = userAssignmentsStats.Message,
                Data = null
            };
        }

        return new ResponseView<StudentDashboardHeaderInfoViewModel>
        {
            Code = StatusCodesEnum.Success,
            Data = new StudentDashboardHeaderInfoViewModel
            {
                CompletedAssignmentsCount = userAssignmentsStats.Data.CompletedAssignmentsCount,
                InProgressAssignmentsCount = userAssignmentsStats.Data.InProgressAssignmentsCount,
                PendingAssignmentsCount = userAssignmentsStats.Data.PendingAssignmentsCount,
                UserScoresStats = userStats.Data
            }
        };
    }

    public async Task<ResponseView<UserPerformanceViewModel>> GetUserPerformanceAsync(int userId,
        DateTime? fromDate = null, DateTime? toDate = null)
    {
        return await assignmentRepository.GetUserPerformance(userId, fromDate, toDate);
    }

    public async Task<ResponseView<TeacherDashboardBaseInfo>> GetTeacherDashboardHeaderInfoAsync(int teacherId)
    {
        var groupsCount = await groupRepository.TeacherGroupsCount(teacherId);
        if(groupsCount.Code != StatusCodesEnum.Success)
        {
            return new ResponseView<TeacherDashboardBaseInfo>
            {
                Code = groupsCount.Code,
                Message = groupsCount.Message,
                Data = null
            };
        }
        var enrolledStudentsCount = await groupRepository.TeacherGroupsEnrolledStudentsCount(teacherId);
        if(enrolledStudentsCount.Code != StatusCodesEnum.Success)
        {
            return new ResponseView<TeacherDashboardBaseInfo>
            {
                Code = enrolledStudentsCount.Code,
                Message = enrolledStudentsCount.Message,
                Data = null
            };
        }
        var createdAssignmentsCount = await assignmentRepository.GetTeacherCreatedAssignmentsCount(teacherId);
        if(createdAssignmentsCount.Code != StatusCodesEnum.Success)
        {
            return new ResponseView<TeacherDashboardBaseInfo>
            {
                Code = createdAssignmentsCount.Code,
                Message = createdAssignmentsCount.Message,
                Data = null
            };
        }
        var needToEvaluateAssignmentsCount = await assignmentRepository.GetTeacherNeedToEvaluateAssignmentsCount(teacherId);
        if(needToEvaluateAssignmentsCount.Code != StatusCodesEnum.Success)
        {
            return new ResponseView<TeacherDashboardBaseInfo>
            {
                Code = needToEvaluateAssignmentsCount.Code,
                Message = needToEvaluateAssignmentsCount.Message,
                Data = null
            };
        }
        return new ResponseView<TeacherDashboardBaseInfo>
        {
            Code = StatusCodesEnum.Success,
            Data = new TeacherDashboardBaseInfo
            {
                GroupsCount = groupsCount.Data,
                EnrolledStudentsCount = enrolledStudentsCount.Data,
                AssignmentsCount = createdAssignmentsCount.Data,
                NeedEvaluateAssignmentsCount = needToEvaluateAssignmentsCount.Data
            }
        };
    }

    public Task<ResponseView<GroupsPerformance>> GetTeacherGroupsPerformanceByDate(int teacherId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        return assignmentRepository.GetTeacherGroupsPerformanceByDate(teacherId, fromDate, toDate);
    }
}