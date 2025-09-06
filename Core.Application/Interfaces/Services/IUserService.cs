using Core.Application.Models.ReturnViewModels;

namespace Core.Application.Interfaces.Services;

public interface IUserService
{
    Task<ResponseView<UserModal>> GetUserDefaultInfoAsync(int userId);

    Task<ResponseView<StudentDashboardHeaderInfoViewModel>> GetStudentDashboardHeaderInfoAsync(int userId);
    
    Task<ResponseView<UserPerformanceViewModel>> GetUserPerformanceAsync(int userId, DateTime? fromDate = null, DateTime? toDate = null);
    Task<ResponseView<TeacherDashboardBaseInfo>> GetTeacherDashboardHeaderInfoAsync(int teacherId);
    Task<ResponseView<GroupsPerformance>> GetTeacherGroupsPerformanceByDate(int teacherId, DateTime? fromDate = null, DateTime? toDate = null);
}