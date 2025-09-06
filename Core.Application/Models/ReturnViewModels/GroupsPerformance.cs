namespace Core.Application.Models.ReturnViewModels;

public class GroupsPerformance
{
    public List<GroupByDatePerformanceStat> GroupsPerformanceStatsByDate { get; set; } = [];
}
public class GroupByDatePerformanceStat
{
    public DateTime DateTime { get; set; }
    public List<GroupPerformanceStat> GroupsPerformanceStats { get; set; } = [];
}

public class GroupPerformanceStat
{
    public GroupBaseModal Group { get; set; }
    public double TotalScoreAvg { get; set; }
    public double GrammarScoreAvg { get; set; }
    public double FluencyScoreAvg { get; set; }
}