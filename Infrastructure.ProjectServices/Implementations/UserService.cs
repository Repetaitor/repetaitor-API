using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.Models;

namespace Infrastructure.ProjectServices.Implementations;

public class UserService(IUserRepository userRepository, IAssignmentRepository assignmentRepository) : IUserService
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

    public async Task<ResponseView<UserPerformanceViewModel>> GetUserPerformanceAsync(int userId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        return await assignmentRepository.GetUserPerformance(userId, fromDate, toDate);
    }
}