namespace Core.Application.Models.ReturnViewModels;

public class TeacherDashboardBaseInfo
{
    public int GroupsCount { get; set; }
    public int EnrolledStudentsCount { get; set; }
    public int AssignmentsCount { get; set; }
    public int EssayCount { get; set; }
    public int NeedEvaluateAssignmentsCount { get; set; }
    public List<PerformanceStat> GroupPerformanceStats { get; set; } = [];
}