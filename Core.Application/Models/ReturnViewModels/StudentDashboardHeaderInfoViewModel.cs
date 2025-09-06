namespace Core.Application.Models.ReturnViewModels;

public class StudentDashboardHeaderInfoViewModel
{
    public int CompletedAssignmentsCount { get; set; }
    public int InProgressAssignmentsCount { get; set; }
    public int PendingAssignmentsCount { get; set; }
    public UserScoresStatsModel UserScoresStats { get; set; }
    public List<PerformanceStat> UserPerformanceStats { get; set; } = [];
}