using Application.Models;
using Core.Application.Models;

namespace Core.Application.Interfaces.Services;

public interface IUserService
{
    Task<ResponseView<UserModal>> GetUserDefaultInfoAsync(int userId);

    Task<ResponseView<StudentDashboardHeaderInfoViewModel>> GetStudentDashboardHeaderInfoAsync(int userId);
    
    Task<ResponseView<UserPerformanceViewModel>> GetUserPerformanceAsync(int userId, DateTime? fromDate = null, DateTime? toDate = null);
}