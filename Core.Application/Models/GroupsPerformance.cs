using System.Text.RegularExpressions;

namespace Core.Application.Models;

public class GroupsPerformance
{
    public List<GroupPerformanceStat> GroupPerformanceStats { get; set; } = [];
}
public class GroupPerformanceStat
{
    public GroupBaseModal Group { get; set; }
    public DateTime DateTime { get; set; }
    public double TotalScoreAvg { get; set; }
    public double GrammarScoreAvg { get; set; }
    public double FluencyScoreAvg { get; set; }
}